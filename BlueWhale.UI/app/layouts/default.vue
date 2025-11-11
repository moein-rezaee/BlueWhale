<template>
  <div class="flex h-screen bg-gray-950">
    <!-- Sidebar -->
    <div class="fixed left-0 top-0 h-screen w-64 bg-gradient-to-b from-slate-900 to-gray-900 border-r border-slate-800 flex flex-col z-50">
      <!-- Logo -->
      <div class="p-6 border-b border-slate-800">
        <NuxtLink to="/" class="flex items-center gap-3 hover:opacity-80 transition">
          <div class="w-10 h-10 bg-gradient-to-br from-blue-500 to-blue-600 rounded-lg flex items-center justify-center shadow-lg shadow-blue-500/50">
            <i class="fas fa-water text-xl text-white"></i>
          </div>
          <div>
            <h1 class="text-lg font-bold text-white">BlueWhale</h1>
            <p class="text-xs text-gray-400">Registry v1.0</p>
          </div>
        </NuxtLink>
      </div>

      <!-- Navigation -->
      <nav class="flex-1 p-4 space-y-1 overflow-y-auto">
        <NuxtLink
          to="/"
          class="flex items-center gap-3 px-4 py-3 rounded-lg cursor-pointer transition-all duration-200"
          :class="{ 
            'bg-gradient-to-r from-blue-600 to-blue-700 text-white shadow-lg shadow-blue-500/30': isActive('/'), 
            'text-gray-400 hover:text-white hover:bg-slate-800/50': !isActive('/')
          }"
        >
          <i class="fas fa-chart-line w-5"></i>
          <span class="font-medium">Dashboard</span>
        </NuxtLink>

        <NuxtLink
          to="/repositories"
          class="flex items-center gap-3 px-4 py-3 rounded-lg cursor-pointer transition-all duration-200"
          :class="{ 
            'bg-gradient-to-r from-green-600 to-green-700 text-white shadow-lg shadow-green-500/30': isActive('/repositories'), 
            'text-gray-400 hover:text-white hover:bg-slate-800/50': !isActive('/repositories')
          }"
        >
          <i class="fas fa-box w-5"></i>
          <span class="font-medium">Repositories</span>
        </NuxtLink>

        <NuxtLink
          to="/security"
          class="flex items-center gap-3 px-4 py-3 rounded-lg cursor-pointer transition-all duration-200"
          :class="{ 
            'bg-gradient-to-r from-purple-600 to-purple-700 text-white shadow-lg shadow-purple-500/30': isActive('/security'), 
            'text-gray-400 hover:text-white hover:bg-slate-800/50': !isActive('/security')
          }"
        >
          <i class="fas fa-shield-alt w-5"></i>
          <span class="font-medium">Access Control</span>
        </NuxtLink>

        <NuxtLink
          to="/activity"
          class="flex items-center gap-3 px-4 py-3 rounded-lg cursor-pointer transition-all duration-200"
          :class="{ 
            'bg-gradient-to-r from-yellow-600 to-yellow-700 text-white shadow-lg shadow-yellow-500/30': isActive('/activity'), 
            'text-gray-400 hover:text-white hover:bg-slate-800/50': !isActive('/activity')
          }"
        >
          <i class="fas fa-history w-5"></i>
          <span class="font-medium">Activity Logs</span>
        </NuxtLink>

        <NuxtLink
          to="/settings"
          class="flex items-center gap-3 px-4 py-3 rounded-lg cursor-pointer transition-all duration-200"
          :class="{ 
            'bg-gradient-to-r from-red-600 to-red-700 text-white shadow-lg shadow-red-500/30': isActive('/settings'), 
            'text-gray-400 hover:text-white hover:bg-slate-800/50': !isActive('/settings')
          }"
        >
          <i class="fas fa-cog w-5"></i>
          <span class="font-medium">Settings</span>
        </NuxtLink>
      </nav>

      <!-- Footer -->
      <div class="p-4 border-t border-slate-800 bg-slate-800/30">
        <div class="flex items-center gap-3 px-4 py-3 bg-slate-700/50 rounded-lg hover:bg-slate-700 transition cursor-pointer">
          <div class="w-10 h-10 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full flex items-center justify-center text-sm font-bold">
            AB
          </div>
          <div class="flex-1">
            <p class="text-sm font-medium text-white">Admin</p>
            <p class="text-xs text-gray-400">administrator</p>
          </div>
          <i class="fas fa-ellipsis-v text-gray-400 text-xs"></i>
        </div>
      </div>
    </div>

    <!-- Main Content Area -->
    <div class="ml-64 flex-1 flex flex-col">
      <!-- Top Header -->
      <header class="bg-gradient-to-r from-slate-800 to-slate-900 border-b border-slate-700 sticky top-0 z-40 backdrop-blur-sm">
        <div class="px-8 py-4">
          <div class="flex items-center justify-between">
            <div class="flex-1 max-w-xl">
              <div class="relative">
                <i class="fas fa-search absolute left-4 top-1/2 transform -translate-y-1/2 text-gray-500"></i>
                <input
                  type="text"
                  placeholder="Search repositories, tags, images..."
                  class="w-full bg-slate-700/50 border border-slate-600 rounded-lg pl-12 pr-4 py-2.5 text-sm text-white placeholder-gray-400 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition"
                >
              </div>
            </div>
            <div class="flex items-center gap-6 ml-8">
              <button 
                @click="refreshPage"
                class="p-2 text-gray-400 hover:text-white hover:bg-slate-700/50 rounded-lg transition-all duration-200 group relative"
                title="Refresh"
              >
                <i :class="['fas', isRefreshing ? 'fa-spinner fa-spin' : 'fa-sync-alt']"></i>
                <span class="absolute -bottom-8 left-1/2 transform -translate-x-1/2 bg-slate-700 text-white text-xs py-1 px-2 rounded whitespace-nowrap opacity-0 group-hover:opacity-100 transition pointer-events-none">Refresh</span>
              </button>
              
              <!-- Theme Toggle -->
              <button 
                @click="toggleTheme"
                class="p-2 text-gray-400 hover:text-white hover:bg-slate-700/50 rounded-lg transition-all duration-200 group relative"
                :title="theme === 'dark' ? 'Switch to Light Mode' : 'Switch to Dark Mode'"
              >
                <i :class="theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon'"></i>
                <span class="absolute -bottom-8 left-1/2 transform -translate-x-1/2 bg-slate-700 text-white text-xs py-1 px-2 rounded whitespace-nowrap opacity-0 group-hover:opacity-100 transition pointer-events-none">
                  {{ theme === 'dark' ? 'Light Mode' : 'Dark Mode' }}
                </span>
              </button>
              
              <button 
                class="relative p-2 text-gray-400 hover:text-white hover:bg-slate-700/50 rounded-lg transition-all duration-200"
                title="Notifications"
              >
                <i class="fas fa-bell"></i>
                <span class="absolute top-0 right-0 w-2.5 h-2.5 bg-red-500 rounded-full animate-pulse"></span>
              </button>
              <div class="w-8 h-8 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center text-sm font-bold cursor-pointer hover:shadow-lg hover:shadow-blue-500/50 transition" title="Account">
                A
              </div>
            </div>
          </div>
        </div>
      </header>

      <!-- Page Content -->
      <main class="flex-1 overflow-y-auto p-8">
        <slot />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useTheme } from '~/composables/useTheme'

const route = useRoute()
const isRefreshing = ref(false)
const { theme, toggleTheme, initTheme } = useTheme()

onMounted(() => {
  initTheme()
})

const isActive = (path: string) => {
  if (path === '/') {
    return route.path === '/'
  }
  return route.path.startsWith(path)
}

const refreshPage = async () => {
  isRefreshing.value = true
  await new Promise(resolve => setTimeout(resolve, 500))
  isRefreshing.value = false
  location.reload()
}
</script>

<style scoped>
/* Custom scrollbar */
::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

::-webkit-scrollbar-track {
  background: rgba(30, 41, 59, 0.5);
  border-radius: 4px;
}

::-webkit-scrollbar-thumb {
  background: rgba(71, 85, 105, 0.7);
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: rgba(100, 116, 139, 0.9);
}
</style>
