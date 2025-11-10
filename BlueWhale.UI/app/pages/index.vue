<template>
  <div class="min-h-screen bg-gradient-to-br from-gray-900 via-slate-900 to-gray-900 text-gray-100">
    <!-- Header -->
    <div class="mb-8">
      <h1 class="text-4xl font-bold text-white mb-2 flex items-center gap-3">
        <i class="fas fa-chart-line text-blue-400"></i>
        Dashboard
      </h1>
      <p class="text-gray-400">Welcome to BlueWhale Registry Management Panel</p>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <div class="bg-gradient-to-br from-slate-800 to-slate-700 border border-slate-600 rounded-lg p-6 hover:border-blue-400 transition">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-400 text-sm font-medium">Total Repositories</p>
            <p class="text-3xl font-bold text-white mt-2">{{ summary.totalRepositories }}</p>
          </div>
          <i class="fas fa-box text-4xl text-blue-400 opacity-30"></i>
        </div>
      </div>

      <div class="bg-gradient-to-br from-slate-800 to-slate-700 border border-slate-600 rounded-lg p-6 hover:border-green-400 transition">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-400 text-sm font-medium">Total Tags</p>
            <p class="text-3xl font-bold text-white mt-2">{{ summary.totalTags }}</p>
          </div>
          <i class="fas fa-tags text-4xl text-green-400 opacity-30"></i>
        </div>
      </div>

      <div class="bg-gradient-to-br from-slate-800 to-slate-700 border border-slate-600 rounded-lg p-6 hover:border-purple-400 transition">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-400 text-sm font-medium">Total Size</p>
            <p class="text-2xl font-bold text-white mt-2">{{ formatBytes(summary.totalSize) }}</p>
          </div>
          <i class="fas fa-database text-4xl text-purple-400 opacity-30"></i>
        </div>
      </div>

      <div class="bg-gradient-to-br from-slate-800 to-slate-700 border border-slate-600 rounded-lg p-6 hover:border-yellow-400 transition">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-400 text-sm font-medium">Status</p>
            <p class="text-2xl font-bold text-white mt-2 flex items-center gap-2">
              <span class="w-3 h-3 bg-green-500 rounded-full inline-block animate-pulse"></span>
              Healthy
            </p>
          </div>
          <i class="fas fa-heartbeat text-4xl text-yellow-400 opacity-30"></i>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="bg-gradient-to-br from-slate-800 to-slate-700 border border-slate-600 rounded-lg p-6 mb-8">
      <h2 class="text-xl font-bold text-white mb-4 flex items-center gap-2">
        <i class="fas fa-zap text-yellow-400"></i>
        Quick Actions
      </h2>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <button 
          @click="$router.push('/repositories')"
          class="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded-lg transition-colors flex items-center gap-2 justify-center"
        >
          <i class="fas fa-list"></i>
          View Repositories
        </button>
        <button 
          @click="refreshSummary"
          class="bg-slate-700 hover:bg-slate-600 text-white font-medium py-2 px-4 rounded-lg transition-colors flex items-center gap-2 justify-center"
        >
          <i :class="isLoading ? 'fas fa-spinner fa-spin' : 'fas fa-sync'"></i>
          Refresh Data
        </button>
        <button 
          @click="downloadReport"
          class="bg-slate-700 hover:bg-slate-600 text-white font-medium py-2 px-4 rounded-lg transition-colors flex items-center gap-2 justify-center"
        >
          <i class="fas fa-download"></i>
          Download Report
        </button>
      </div>
    </div>

    <!-- Top Repositories -->
    <div v-if="topRepositories.length > 0" class="bg-gradient-to-br from-slate-800 to-slate-700 border border-slate-600 rounded-lg p-6 mb-8">
      <h2 class="text-xl font-bold text-white mb-4 flex items-center gap-2">
        <i class="fas fa-chart-bar text-blue-400"></i>
        Top Repositories by Size
      </h2>
      <div class="space-y-3">
        <div 
          v-for="repo in topRepositories" 
          :key="repo.name"
          class="p-4 bg-slate-700/50 rounded-lg hover:bg-slate-700 transition"
        >
          <div class="flex items-center justify-between mb-2">
            <div class="flex items-center gap-2">
              <i class="fas fa-box text-blue-400"></i>
              <span class="font-medium">{{ repo.name }}</span>
            </div>
            <span class="text-sm text-gray-400">{{ repo.tagCount }} tags</span>
          </div>
          <div class="flex items-center gap-2">
            <div class="flex-1 bg-slate-600 rounded-full h-2 overflow-hidden">
              <div 
                class="h-full bg-gradient-to-r from-blue-500 to-blue-600"
                :style="{ width: calculateWidth(repo) + '%' }"
              ></div>
            </div>
            <span class="text-sm text-gray-400 w-20 text-right">{{ formatBytes(repo.totalSize) }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="text-center py-12">
      <i class="fas fa-spinner fa-spin text-4xl text-blue-400 mb-4"></i>
      <p class="text-gray-400">Loading registry data...</p>
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
}

interface Summary {
  totalRepositories: number
  totalTags: number
  totalSize: number
  timestamp: string
}

const config = useRuntimeConfig()
const apiBase = config.public.apiBase
const router = useRouter()

const isLoading = ref(false)
const summary = ref<Summary>({
  totalRepositories: 0,
  totalTags: 0,
  totalSize: 0,
  timestamp: new Date().toISOString()
})

const repositories = ref<Repository[]>([])

const topRepositories = computed(() => {
  return [...repositories.value]
    .sort((a, b) => b.totalSize - a.totalSize)
    .slice(0, 5)
})

function formatBytes(bytes: number): string {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB', 'TB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i]
}

function calculateWidth(repo: Repository): number {
  const maxSize = Math.max(...repositories.value.map(r => r.totalSize), 1)
  return (repo.totalSize / maxSize) * 100
}

async function loadSummary() {
  try {
    isLoading.value = true
    
    const summaryRes = await $fetch<Summary>(`${apiBase}/statistics/summary`)
    summary.value = summaryRes

    const reposRes = await $fetch<Repository[]>(`${apiBase}/statistics/repositories`)
    repositories.value = reposRes || []
  } catch (error) {
    console.error('Error loading data:', error)
  } finally {
    isLoading.value = false
  }
}

async function refreshSummary() {
  await loadSummary()
}

function downloadReport() {
  const data = {
    timestamp: new Date().toISOString(),
    summary: summary.value,
    repositories: repositories.value
  }
  
  const element = document.createElement('a')
  element.setAttribute('href', 'data:text/json;charset=utf-8,' + encodeURIComponent(JSON.stringify(data, null, 2)))
  element.setAttribute('download', `registry-report-${new Date().getTime()}.json`)
  element.style.display = 'none'
  document.body.appendChild(element)
  element.click()
  document.body.removeChild(element)
}

onMounted(() => {
  loadSummary()
})
</script>
