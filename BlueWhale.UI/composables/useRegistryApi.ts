const apiBase = '/v1/api'

export const useRegistryApi = () => {
  const baseURL = 'http://localhost:5260'

  const getRepositories = async (): Promise<any[]> => {
    try {
      const response = await fetch(`${baseURL}${apiBase}/repositories`)
      if (!response.ok) throw new Error('Failed to fetch repositories')
      return await response.json()
    } catch (error) {
      console.error('Error fetching repositories:', error)
      return []
    }
  }

  const getRepository = async (name: string): Promise<any | null> => {
    try {
      const response = await fetch(`${baseURL}${apiBase}/repositories/${name}`)
      if (!response.ok) throw new Error('Failed to fetch repository')
      return await response.json()
    } catch (error) {
      console.error(`Error fetching repository ${name}:`, error)
      return null
    }
  }

  const deleteRepository = async (name: string): Promise<boolean> => {
    try {
      const response = await fetch(`${baseURL}${apiBase}/repositories/${name}`, {
        method: 'DELETE'
      })
      return response.ok
    } catch (error) {
      console.error(`Error deleting repository ${name}:`, error)
      return false
    }
  }

  const getTags = async (repository: string): Promise<any[]> => {
    try {
      const response = await fetch(`${baseURL}${apiBase}/tags/${repository}`)
      if (!response.ok) throw new Error('Failed to fetch tags')
      return await response.json()
    } catch (error) {
      console.error(`Error fetching tags for ${repository}:`, error)
      return []
    }
  }

  const getTag = async (repository: string, tag: string): Promise<any | null> => {
    try {
      const response = await fetch(`${baseURL}${apiBase}/tags/${repository}/${tag}`)
      if (!response.ok) throw new Error('Failed to fetch tag')
      return await response.json()
    } catch (error) {
      console.error(`Error fetching tag ${tag} in ${repository}:`, error)
      return null
    }
  }

  const deleteTag = async (repository: string, tag: string): Promise<boolean> => {
    try {
      const response = await fetch(`${baseURL}${apiBase}/tags/${repository}/${tag}`, {
        method: 'DELETE'
      })
      return response.ok
    } catch (error) {
      console.error(`Error deleting tag ${tag} in ${repository}:`, error)
      return false
    }
  }

  return {
    getRepositories,
    getRepository,
    deleteRepository,
    getTags,
    getTag,
    deleteTag
  }
}
