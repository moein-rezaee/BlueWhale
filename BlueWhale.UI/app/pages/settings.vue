<template>
  <div class="space-y-8">
    <!-- Header -->
    <div>
      <h1 class="text-4xl font-bold text-white mb-2">Settings</h1>
      <p class="text-gray-400">Configure registry and system settings</p>
    </div>

    <!-- Registry Settings -->
    <div class="bg-gray-900 border border-gray-800 rounded-lg p-6">
      <h2 class="text-xl font-bold text-white mb-6">Registry Configuration</h2>

      <div class="space-y-6">
        <div>
          <label class="block text-sm font-medium text-gray-300 mb-2">Registry URL</label>
          <input
            type="text"
            v-model="settings.registryUrl"
            class="w-full bg-gray-800 border border-gray-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-blue-500"
          >
          <p class="text-xs text-gray-400 mt-1">The URL where Docker Registry is running</p>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-300 mb-2">Storage Backend</label>
          <select class="w-full bg-gray-800 border border-gray-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-blue-500">
            <option>Filesystem (Default)</option>
            <option>S3</option>
            <option>GCS</option>
            <option>Azure Blob Storage</option>
          </select>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-300 mb-2">Max Upload Size (MB)</label>
            <input
              type="number"
              v-model="settings.maxUploadSize"
              class="w-full bg-gray-800 border border-gray-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-blue-500"
            >
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-300 mb-2">Cleanup Policy (days)</label>
            <input
              type="number"
              v-model="settings.cleanupDays"
              class="w-full bg-gray-800 border border-gray-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-blue-500"
            >
          </div>
        </div>
      </div>
    </div>

    <!-- Security Settings -->
    <div class="bg-gray-900 border border-gray-800 rounded-lg p-6">
      <h2 class="text-xl font-bold text-white mb-6">Security</h2>

      <div class="space-y-6">
        <div class="flex items-center justify-between p-4 bg-gray-800 rounded-lg">
          <div>
            <p class="text-white font-medium">Enable HTTPS</p>
            <p class="text-gray-400 text-sm">Enforce HTTPS for all connections</p>
          </div>
          <label class="flex items-center cursor-pointer">
            <input type="checkbox" v-model="settings.enableHttps" class="sr-only peer">
            <div class="relative w-11 h-6 bg-gray-700 peer-focus:outline-none peer-focus:ring-2 peer-focus:ring-blue-500 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
          </label>
        </div>

        <div class="flex items-center justify-between p-4 bg-gray-800 rounded-lg">
          <div>
            <p class="text-white font-medium">Require Authentication</p>
            <p class="text-gray-400 text-sm">Require login for all operations</p>
          </div>
          <label class="flex items-center cursor-pointer">
            <input type="checkbox" v-model="settings.requireAuth" class="sr-only peer" checked>
            <div class="relative w-11 h-6 bg-gray-700 peer-focus:outline-none peer-focus:ring-2 peer-focus:ring-blue-500 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
          </label>
        </div>

        <div class="flex items-center justify-between p-4 bg-gray-800 rounded-lg">
          <div>
            <p class="text-white font-medium">Enable Delete</p>
            <p class="text-gray-400 text-sm">Allow deletion of images and tags</p>
          </div>
          <label class="flex items-center cursor-pointer">
            <input type="checkbox" v-model="settings.enableDelete" class="sr-only peer" checked>
            <div class="relative w-11 h-6 bg-gray-700 peer-focus:outline-none peer-focus:ring-2 peer-focus:ring-blue-500 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
          </label>
        </div>
      </div>
    </div>

    <!-- Proxy Settings -->
    <div class="bg-gray-900 border border-gray-800 rounded-lg p-6">
      <h2 class="text-xl font-bold text-white mb-6">Proxy Configuration</h2>

      <div class="space-y-6">
        <div>
          <label class="block text-sm font-medium text-gray-300 mb-2">Proxy Mode</label>
          <select class="w-full bg-gray-800 border border-gray-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-blue-500">
            <option>None</option>
            <option>Single Upstream</option>
            <option>Multiple Upstreams (Group)</option>
            <option>Pull Through Cache</option>
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-300 mb-2">Upstream Registries</label>
          <textarea
            v-model="settings.upstreamRegistries"
            rows="3"
            class="w-full bg-gray-800 border border-gray-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-blue-500"
            placeholder="One registry URL per line"
          ></textarea>
        </div>
      </div>
    </div>

    <!-- Action Buttons -->
    <div class="flex gap-4">
      <button class="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-6 rounded-lg transition-colors">
        Save Settings
      </button>
      <button class="bg-gray-800 hover:bg-gray-700 text-white font-medium py-2 px-6 rounded-lg transition-colors">
        Cancel
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

interface Settings {
  registryUrl: string
  maxUploadSize: number
  cleanupDays: number
  enableHttps: boolean
  requireAuth: boolean
  enableDelete: boolean
  upstreamRegistries: string
}

const settings = ref<Settings>({
  registryUrl: 'http://localhost:5000',
  maxUploadSize: 5120,
  cleanupDays: 30,
  enableHttps: false,
  requireAuth: true,
  enableDelete: true,
  upstreamRegistries: 'https://registry-1.docker.io\nhttps://index.docker.io'
})
</script>
