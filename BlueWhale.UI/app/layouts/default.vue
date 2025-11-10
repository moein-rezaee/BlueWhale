<template>
  <div class="flex h-screen bg-gray-950">
    <!-- Sidebar -->
    <div class="fixed left-0 top-0 h-screen w-64 bg-gray-900 border-r border-gray-800 flex flex-col z-50">
      <div class="p-6 border-b border-gray-800">
        <div class="flex items-center gap-3">
          <div class="w-10 h-10 bg-blue-600 rounded-lg flex items-center justify-center">
            <i class="fab fa-docker text-xl text-white"></i>
          </div>
          <div>
            <h1 class="text-lg font-bold text-white">Registry Panel</h1>
            <p class="text-xs text-gray-400">v1.0.0</p>
          </div>
        </div>
      </div>

      <nav class="flex-1 p-4 space-y-1 overflow-y-auto">
        <NuxtLink
          to="/"
          class="flex items-center gap-3 px-4 py-3 rounded-lg cursor-pointer transition-all"
          :class="{ 'bg-blue-600 text-white': $route.path === '/', 'text-gray-400 hover:bg-gray-800': $route.path !== '/' }"
        >
          <i class="fas fa-chart-line w-5"></i>
          <span class="font-medium">Dashboard</span>
        </NuxtLink>

        <NuxtLink
          to="/repositories"
          class="flex items-center gap-3 px-4 py-3 rounded-lg cursor-pointer transition-all"
          :class="{ 'bg-blue-600 text-white': $route.path.startsWith('/repositories'), 'text-gray-400 hover:bg-gray-800': !$route.path.startsWith('/repositories') }"
        >
          <i class="fas fa-box w-5"></i>
          <span class="font-medium">Repositories</span>
        </NuxtLink>

        <NuxtLink
          to="/security"
          class="flex items-center gap-3 px-4 py-3 rounded-lg cursor-pointer transition-all"
          :class="{ 'bg-blue-600 text-white': $route.path === '/security', 'text-gray-400 hover:bg-gray-800': $route.path !== '/security' }"
        >
          <i class="fas fa-shield-alt w-5"></i>
          <span class="font-medium">Access Control</span>
        </NuxtLink>

        <NuxtLink
          to="/activity"
          class="flex items-center gap-3 px-4 py-3 rounded-lg cursor-pointer transition-all"
          :class="{ 'bg-blue-600 text-white': $route.path === '/activity', 'text-gray-400 hover:bg-gray-800': $route.path !== '/activity' }"
        >
          <i class="fas fa-file-alt w-5"></i>
          <span class="font-medium">Activity Logs</span>
        </NuxtLink>

        <NuxtLink
          to="/settings"
          class="flex items-center gap-3 px-4 py-3 rounded-lg cursor-pointer transition-all"
          :class="{ 'bg-blue-600 text-white': $route.path === '/settings', 'text-gray-400 hover:bg-gray-800': $route.path !== '/settings' }"
        >
          <i class="fas fa-cog w-5"></i>
          <span class="font-medium">Settings</span>
        </NuxtLink>
      </nav>

      <div class="p-4 border-t border-gray-800">
        <div class="flex items-center gap-3 px-4 py-3 bg-gray-800 rounded-lg">
          <div class="w-10 h-10 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full flex items-center justify-center text-sm font-bold">
            AD
          </div>
          <div class="flex-1">
            <p class="text-sm font-medium text-white">Admin</p>
            <p class="text-xs text-gray-400">admin@registry</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="ml-64 flex-1 flex flex-col">
      <!-- Header -->
      <header class="bg-gray-900 border-b border-gray-800 sticky top-0 z-40">
        <div class="px-8 py-4">
          <div class="flex items-center justify-between">
            <div class="flex-1 max-w-xl">
              <div class="relative">
                <i class="fas fa-search absolute left-4 top-1/2 transform -translate-y-1/2 text-gray-400"></i>
                <input
                  type="text"
                  placeholder="Search repositories..."
                  class="w-full bg-gray-800 border border-gray-700 rounded-lg pl-12 pr-4 py-3 text-sm text-white placeholder-gray-400 focus:outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20"
                >
              </div>
            </div>
            <div class="flex items-center gap-4 ml-8">
              <button class="relative p-2 text-gray-400 hover:text-white hover:bg-gray-800 rounded-lg transition-all">
                <i class="fas fa-bell text-xl"></i>
                <span class="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full"></span>
              </button>
              <div class="w-8 h-8 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center text-sm font-bold cursor-pointer">
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
import { useRoute } from 'vue-router'

const route = useRoute()
</script>

<style scoped>
/* Custom scrollbar */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-track {
  background: #111827;
}

::-webkit-scrollbar-thumb {
  background: #374151;
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: #4b5563;
}
</style>
