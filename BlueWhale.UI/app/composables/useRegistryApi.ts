export const useRegistryApi = () => {
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBase || 'http://localhost:5260/v1/api'

  async function request<T>(method: string, endpoint: string, body?: any): Promise<T> {
    const url = `${baseURL}${endpoint}`
    const headers: Record<string, string> = {
      'Content-Type': 'application/json'
    }

    // Add JWT token if available (client-side only)
    try {
      const token = localStorage.getItem('accessToken')
      if (token) {
        headers['Authorization'] = `Bearer ${token}`
      }
    } catch (e) {
      // localStorage not available (SSR), skip token
    }

    const options: RequestInit = {
      method,
      headers
    }

    if (body && (method === 'POST' || method === 'PUT' || method === 'PATCH')) {
      options.body = JSON.stringify(body)
    }

    try {
      const response = await fetch(url, options)
      
      if (response.status === 401) {
        // Unauthorized - clear tokens and redirect to login
        try {
          localStorage.removeItem('accessToken')
          localStorage.removeItem('refreshToken')
          localStorage.removeItem('user')
          if (typeof window !== 'undefined') {
            window.location.href = '/login'
          }
        } catch (e) {
          // localStorage not available
        }
      }

      if (!response.ok) {
        const error = await response.text()
        throw new Error(error || `API request failed with status ${response.status}`)
      }

      const contentType = response.headers.get('content-type')
      if (contentType && contentType.includes('application/json')) {
        return await response.json() as T
      }

      return response as unknown as T
    } catch (error) {
      console.error(`API Error [${method} ${endpoint}]:`, error)
      throw error
    }
  }

  // Statistics
  const getStatisticsSummary = () => request<{
    totalRepositories: number
    totalTags: number
    totalSize: number
    timestamp: string
  }>('GET', '/statistics/summary')

  const getRepositoriesStats = () => request<Array<{
    name: string
    tagCount: number
    totalSize: number
    lastPushed: string
  }>>('GET', '/statistics/repositories')

  // Health
  const getHealth = () => request<{
    status: string
    timestamp: string
    service: string
  }>('GET', '/health')

  // Repositories
  const getRepositories = () => request<Array<{
    name: string
    tagCount: number
    lastPushed: string
  }>>('GET', '/repositories')

  const getRepository = (name: string) => request<{
    name: string
    tagCount: number
    totalSize: number
    lastPushed: string
    tags: Array<{
      name: string
      size: number
      created: string
      digest: string
    }>
  }>('GET', `/repositories/${name}`)

  const deleteRepository = (name: string) => request('DELETE', `/repositories/${name}`)

  // Tags
  const getTags = (repository: string) => request<Array<{
    name: string
    size: number
    created: string
    digest: string
  }>>('GET', `/tags/${repository}`)

  const getTag = (repository: string, tag: string) => request<{
    contentType: string
    digest: string
    config: string
  }>('GET', `/tags/${repository}/${tag}`)

  const deleteTag = (repository: string, tag: string) => request('DELETE', `/tags/${repository}/${tag}`)

  return {
    getStatisticsSummary,
    getRepositoriesStats,
    getHealth,
    getRepositories,
    getRepository,
    deleteRepository,
    getTags,
    getTag,
    deleteTag
  }
}


