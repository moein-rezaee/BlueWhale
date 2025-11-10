<template>
  <div class="min-h-screen bg-gradient-to-br from-gray-900 via-slate-900 to-gray-900 text-gray-100">
    <!-- Header -->
    <div class="flex justify-between items-center mb-8">
      <div>
        <h1 class="text-3xl font-bold text-white flex items-center gap-3 mb-2">
          <i class="fas fa-box text-green-400"></i>
          Repositories
        </h1>
        <p class="text-gray-400">Manage your Docker images and repositories</p>
      </div>
      <button 
        @click="refreshRepositories"
        :disabled="isLoading"
        class="px-6 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors flex items-center gap-2 disabled:opacity-50"
      >
        <i :class="['fas', isLoading ? 'fa-spinner fa-spin' : 'fa-sync-alt']"></i>
        <span>{{ isLoading ? 'Loading...' : 'Refresh' }}</span>
      </button>
    </div>

    <!-- Error Alert -->
    <div v-if="error" class="bg-red-900/30 border border-red-700 text-red-200 px-4 py-3 rounded-lg mb-6 flex items-center gap-2">
      <i class="fas fa-exclamation-circle"></i>
      {{ error }}
    </div>

    <!-- Search Bar -->
    <div class="relative mb-6">
      <i class="fas fa-search absolute left-4 top-3 text-gray-500"></i>
      <input 
        v-model="searchQuery"
        type="text" 
        placeholder="Search repositories..."
        class="w-full pl-10 pr-4 py-2 bg-slate-800 border border-slate-600 rounded-lg focus:outline-none focus:border-blue-500 transition"
      />
    </div>

    <!-- Repositories Grid -->
    <div v-if="!isLoading && filteredRepositories.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div 
        v-for="repo in filteredRepositories" 
        :key="repo.name"
        class="bg-gradient-to-br from-slate-800 to-slate-700 border border-slate-600 hover:border-blue-400 rounded-lg p-6 transition cursor-pointer"
        @click="selectRepository(repo)"
      >
        <div class="flex items-start justify-between mb-4">
          <div class="flex items-center gap-3">
            <i class="fas fa-box text-blue-400 text-2xl"></i>
            <div>
              <h3 class="font-semibold text-lg">{{ repo.name }}</h3>
              <p class="text-sm text-gray-400">{{ repo.tagCount }} tags</p>
            </div>
          </div>
          <div class="flex gap-2">
            <button 
              @click.stop="copyName(repo.name)"
              class="text-gray-400 hover:text-blue-400 transition p-1"
              title="Copy name"
            >
              <i class="fas fa-copy"></i>
            </button>
            <button 
              @click.stop="deleteRepository(repo.name)"
              v-if="!isDeleting[repo.name]"
              class="text-gray-400 hover:text-red-400 transition p-1"
              title="Delete"
            >
              <i class="fas fa-trash"></i>
            </button>
            <i v-else class="fas fa-spinner fa-spin text-yellow-400 p-1"></i>
          </div>
        </div>

        <div class="space-y-2 text-sm mb-4">
          <div class="flex justify-between text-gray-300">
            <span>Size:</span>
            <span class="font-semibold">{{ formatBytes(repo.totalSize) }}</span>
          </div>
          <div class="flex justify-between text-gray-300">
            <span>Last Updated:</span>
            <span class="font-semibold">{{ formatDate(repo.lastPushed) }}</span>
          </div>
        </div>

        <!-- Progress Bar -->
        <div class="bg-slate-700/50 rounded-full h-2 overflow-hidden">
          <div 
            class="h-full bg-gradient-to-r from-blue-500 to-blue-600 transition-all"
            :style="{ width: calculateWidth(repo) + '%' }"
          ></div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-if="!isLoading && filteredRepositories.length === 0 && searchQuery === ''" class="text-center py-16">
      <i class="fas fa-inbox text-6xl text-gray-600 mb-4"></i>
      <p class="text-gray-400 text-lg">No repositories found</p>
      <p class="text-gray-500 text-sm mt-2">Connect to your Docker Registry or push images to get started</p>
    </div>

    <!-- No Search Results -->
    <div v-if="!isLoading && filteredRepositories.length === 0 && searchQuery !== ''" class="text-center py-16">
      <i class="fas fa-search text-6xl text-gray-600 mb-4"></i>
      <p class="text-gray-400 text-lg">No repositories match your search</p>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="text-center py-16">
      <i class="fas fa-spinner fa-spin text-6xl text-blue-400 mb-4"></i>
      <p class="text-gray-400">Loading repositories...</p>
    </div>

    <!-- Repository Details Modal -->
    <div v-if="selectedRepository" class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4">
      <div class="bg-slate-800 border border-slate-600 rounded-lg max-w-2xl w-full max-h-96 overflow-y-auto">
        <div class="sticky top-0 bg-gradient-to-r from-slate-700 to-slate-800 px-6 py-4 border-b border-slate-600 flex justify-between items-center">
          <h2 class="text-xl font-semibold flex items-center gap-3">
            <i class="fas fa-box text-blue-400"></i>
            {{ selectedRepository.name }}
          </h2>
          <button 
            @click="selectedRepository = null"
            class="text-gray-400 hover:text-gray-200 transition"
          >
            <i class="fas fa-times text-2xl"></i>
          </button>
        </div>

        <div class="p-6 space-y-4">
          <div class="grid grid-cols-3 gap-4">
            <div>
              <p class="text-gray-400 text-sm">Tags</p>
              <p class="text-2xl font-bold text-blue-400">{{ selectedRepository.tagCount }}</p>
            </div>
            <div>
              <p class="text-gray-400 text-sm">Size</p>
              <p class="text-xl font-bold text-blue-400">{{ formatBytes(selectedRepository.totalSize) }}</p>
            </div>
            <div>
              <p class="text-gray-400 text-sm">Status</p>
              <p class="text-lg font-bold text-green-400">Active</p>
            </div>
          </div>

          <!-- Tags List -->
          <div v-if="selectedRepository.tags && selectedRepository.tags.length > 0" class="space-y-3">
            <h3 class="font-semibold text-lg">Image Tags</h3>
            <div class="space-y-2 max-h-48 overflow-y-auto">
              <div 
                v-for="tag in selectedRepository.tags" 
                :key="tag.name"
                class="flex justify-between items-center bg-slate-700/50 p-3 rounded-lg hover:bg-slate-700 transition"
              >
                <div class="flex items-center gap-2">
                  <i class="fas fa-tag text-green-400 text-sm"></i>
                  <div>
                    <p class="text-sm font-medium">{{ tag.name }}</p>
                    <p class="text-xs text-gray-400">{{ formatBytes(tag.size) }}</p>
                  </div>
                </div>
                <button
                  @click="deleteTag(selectedRepository.name, tag.name)"
                  v-if="!isDeletingTag[tag.name]"
                  class="text-red-400 hover:text-red-300 transition text-sm"
                >
                  <i class="fas fa-trash"></i>
                </button>
                <i v-else class="fas fa-spinner fa-spin text-yellow-400 text-sm"></i>
              </div>
            </div>
          </div>

          <div v-else class="text-center py-8 text-gray-400">
            <i class="fas fa-inbox text-2xl mb-2"></i>
            <p>No tags found</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'

interface Repository {
  name: string
  tagCount: number
  totalSize: number
  lastPushed: string
  tags?: Tag[]
}

interface Tag {
  name: string
  size: number
  created: string
  digest: string
}

const config = useRuntimeConfig()
const apiBase = config.public.apiBase

const repositories = ref<Repository[]>([])
const isLoading = ref(false)
const error = ref<string | null>(null)
const searchQuery = ref('')
const selectedRepository = ref<Repository | null>(null)
const isDeleting = ref<Record<string, boolean>>({})
const isDeletingTag = ref<Record<string, boolean>>({})

const filteredRepositories = computed(() => {
  return repositories.value.filter(repo =>
    repo.name.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
})

function formatBytes(bytes: number): string {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB', 'TB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
}

function formatDate(date: string | undefined): string {
  if (!date) return 'N/A'
  try {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    })
  } catch {
    return 'Unknown'
  }
}

function calculateWidth(repo: Repository): number {
  const maxSize = Math.max(...repositories.value.map(r => r.totalSize), 1)
  return (repo.totalSize / maxSize) * 100
}

function copyName(name: string) {
  navigator.clipboard.writeText(name)
}

async function refreshRepositories() {
  try {
    isLoading.value = true
    error.value = null
    const res = await $fetch<Repository[]>(`${apiBase}/statistics/repositories`)
    repositories.value = res || []
  } catch (err) {
    error.value = 'Failed to load repositories'
    console.error(err)
  } finally {
    isLoading.value = false
  }
}

async function deleteRepository(name: string) {
  if (!confirm(`Are you sure you want to delete repository "${name}"?`)) return

  try {
    isDeleting.value[name] = true
    await $fetch(`${apiBase}/repositories/${name}`, { method: 'DELETE' })
    repositories.value = repositories.value.filter(r => r.name !== name)
  } catch (err) {
    error.value = `Failed to delete repository: ${name}`
    console.error(err)
  } finally {
    isDeleting.value[name] = false
  }
}

async function deleteTag(repoName: string, tagName: string) {
  if (!confirm(`Are you sure you want to delete tag "${tagName}"?`)) return

  try {
    isDeletingTag.value[tagName] = true
    await $fetch(`${apiBase}/tags/${repoName}/${tagName}`, { method: 'DELETE' })
    
    if (selectedRepository.value) {
      const updated = await $fetch<Repository>(`${apiBase}/repositories/${repoName}`)
      selectedRepository.value = updated || null
      repositories.value = repositories.value.map(r => r.name === repoName ? (updated || r) : r)
    }
  } catch (err) {
    error.value = `Failed to delete tag: ${tagName}`
    console.error(err)
  } finally {
    isDeletingTag.value[tagName] = false
  }
}

async function selectRepository(repo: Repository) {
  try {
    isLoading.value = true
    const details = await $fetch<Repository>(`${apiBase}/repositories/${repo.name}`)
    selectedRepository.value = details || null
  } catch (err) {
    error.value = `Failed to load repository details for ${repo.name}`
    console.error(err)
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  refreshRepositories()
})
</script>

<style scoped>
::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

::-webkit-scrollbar-track {
  background: rgba(15, 23, 42, 0.5);
  border-radius: 10px;
}

::-webkit-scrollbar-thumb {
  background: rgba(71, 85, 105, 0.7);
  border-radius: 10px;
}

::-webkit-scrollbar-thumb:hover {
  background: rgba(100, 116, 139, 0.7);
}
</style>
