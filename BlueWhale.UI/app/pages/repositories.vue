<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <div>
        <h1 class="text-3xl font-bold text-white">Repositories</h1>
        <p class="text-gray-400 mt-1">Manage your Docker images and repositories</p>
      </div>
      <button 
        @click="refreshRepositories"
        class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors flex items-center gap-2"
      >
        <i :class="['fas', isLoading ? 'fa-spinner fa-spin' : 'fa-redo']"></i>
        <span>{{ isLoading ? 'Loading...' : 'Refresh' }}</span>
      </button>
    </div>

    <div v-if="error" class="bg-red-900/20 border border-red-900 text-red-200 px-4 py-3 rounded-lg">
      <i class="fas fa-exclamation-circle mr-2"></i>{{ error }}
    </div>

    <div class="bg-gray-900 border border-gray-800 rounded-lg overflow-hidden">
      <table class="w-full">
        <thead class="bg-gray-800">
          <tr>
            <th class="px-6 py-3 text-left text-sm font-semibold text-gray-300">Repository</th>
            <th class="px-6 py-3 text-left text-sm font-semibold text-gray-300">Tags</th>
            <th class="px-6 py-3 text-left text-sm font-semibold text-gray-300">Last Updated</th>
            <th class="px-6 py-3 text-right text-sm font-semibold text-gray-300">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-800">
          <tr v-for="repo in repositories" :key="repo.name" class="hover:bg-gray-800/50 transition-colors">
            <td class="px-6 py-4 font-medium text-white">
              <i class="fas fa-box text-gray-500 mr-2"></i>{{ repo.name }}
            </td>
            <td class="px-6 py-4 text-gray-400">{{ repo.tagCount }}</td>
            <td class="px-6 py-4 text-gray-400">{{ formatDate(repo.lastPushed) }}</td>
            <td class="px-6 py-4 text-right space-x-3">
              <button 
                @click="viewRepository(repo.name)"
                class="text-blue-400 hover:text-blue-300 transition-colors"
                title="View details"
              >
                <i class="fas fa-eye"></i>
              </button>
              <button 
                @click="deleteRepository(repo.name)"
                class="text-red-400 hover:text-red-300 transition-colors"
                title="Delete repository"
              >
                <i class="fas fa-trash"></i>
              </button>
            </td>
          </tr>
          <tr v-if="repositories.length === 0">
            <td colspan="4" class="px-6 py-8 text-center text-gray-400">
              <i class="fas fa-inbox text-3xl mb-3 block"></i>
              No repositories found
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default'
})

const baseURL = 'http://localhost:5280'
const apiBase = '/v1/api'

const repositories = ref<any[]>([])
const isLoading = ref(false)
const error = ref<string | null>(null)

const getRepositories = async (): Promise<any[]> => {
  try {
    const response = await fetch(`${baseURL}${apiBase}/repositories`)
    if (!response.ok) throw new Error('Failed to fetch repositories')
    return await response.json()
  } catch (err) {
    console.error('Error fetching repositories:', err)
    return []
  }
}

const deleteRepo = async (name: string): Promise<boolean> => {
  try {
    const response = await fetch(`${baseURL}${apiBase}/repositories/${name}`, {
      method: 'DELETE'
    })
    return response.ok
  } catch (err) {
    console.error(`Error deleting repository ${name}:`, err)
    return false
  }
}

const refreshRepositories = async () => {
  isLoading.value = true
  error.value = null
  try {
    const data = await getRepositories()
    repositories.value = data
  } catch (err) {
    error.value = 'Failed to load repositories'
    console.error(err)
  } finally {
    isLoading.value = false
  }
}

const viewRepository = (name: string) => {
  navigateTo(`/repositories/${name}`)
}

const deleteRepository = async (name: string) => {
  if (!confirm(`Are you sure you want to delete the repository "${name}"?`)) return
  
  try {
    const success = await deleteRepo(name)
    if (success) {
      repositories.value = repositories.value.filter(r => r.name !== name)
    }
  } catch (err) {
    error.value = `Failed to delete repository: ${name}`
  }
}

const formatDate = (date: string | undefined) => {
  if (!date) return 'N/A'
  return new Date(date).toLocaleDateString()
}

onMounted(() => {
  refreshRepositories()
})
</script>
