<template>
  <div class="space-y-8">
    <!-- Header -->
    <div>
      <h1 class="text-4xl font-bold text-white mb-2">Dashboard</h1>
      <p class="text-gray-400">Welcome to BlueWhale Registry Management Panel</p>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div class="bg-gray-900 border border-gray-800 rounded-lg p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-400 text-sm font-medium">Total Repositories</p>
            <p class="text-3xl font-bold text-white mt-2">{{ stats.repositories }}</p>
          </div>
          <i class="fas fa-box text-4xl text-blue-600/30"></i>
        </div>
      </div>

      <div class="bg-gray-900 border border-gray-800 rounded-lg p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-400 text-sm font-medium">Total Images</p>
            <p class="text-3xl font-bold text-white mt-2">{{ stats.images }}</p>
          </div>
          <i class="fas fa-layer-group text-4xl text-purple-600/30"></i>
        </div>
      </div>

      <div class="bg-gray-900 border border-gray-800 rounded-lg p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-400 text-sm font-medium">Total Tags</p>
            <p class="text-3xl font-bold text-white mt-2">{{ stats.tags }}</p>
          </div>
          <i class="fas fa-tags text-4xl text-green-600/30"></i>
        </div>
      </div>

      <div class="bg-gray-900 border border-gray-800 rounded-lg p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-400 text-sm font-medium">Active Users</p>
            <p class="text-3xl font-bold text-white mt-2">{{ stats.users }}</p>
          </div>
          <i class="fas fa-users text-4xl text-orange-600/30"></i>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="bg-gray-900 border border-gray-800 rounded-lg p-6">
      <h2 class="text-xl font-bold text-white mb-4">Quick Actions</h2>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <button class="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded-lg transition-colors flex items-center gap-2">
          <i class="fas fa-plus"></i>
          New Repository
        </button>
        <button class="bg-gray-800 hover:bg-gray-700 text-white font-medium py-2 px-4 rounded-lg transition-colors flex items-center gap-2">
          <i class="fas fa-upload"></i>
          Upload Image
        </button>
        <button class="bg-gray-800 hover:bg-gray-700 text-white font-medium py-2 px-4 rounded-lg transition-colors flex items-center gap-2">
          <i class="fas fa-sync"></i>
          Sync Registry
        </button>
      </div>
    </div>

    <!-- Recent Activity -->
    <div class="bg-gray-900 border border-gray-800 rounded-lg p-6">
      <h2 class="text-xl font-bold text-white mb-4">Recent Activity</h2>
      <div class="space-y-3">
        <div class="flex items-center justify-between p-3 bg-gray-800 rounded-lg">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 bg-blue-600/20 rounded-lg flex items-center justify-center">
              <i class="fas fa-upload text-blue-500"></i>
            </div>
            <div>
              <p class="text-white font-medium">Image pushed to nginx</p>
              <p class="text-gray-400 text-sm">5 minutes ago</p>
            </div>
          </div>
          <span class="px-3 py-1 bg-green-600/20 text-green-400 rounded-full text-sm font-medium">Success</span>
        </div>
        <div class="flex items-center justify-between p-3 bg-gray-800 rounded-lg">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 bg-purple-600/20 rounded-lg flex items-center justify-center">
              <i class="fas fa-trash text-purple-500"></i>
            </div>
            <div>
              <p class="text-white font-medium">Tag deleted from redis</p>
              <p class="text-gray-400 text-sm">1 hour ago</p>
            </div>
          </div>
          <span class="px-3 py-1 bg-green-600/20 text-green-400 rounded-full text-sm font-medium">Success</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const stats = ref({
  repositories: 12,
  images: 45,
  tags: 156,
  users: 8
})
</script>
