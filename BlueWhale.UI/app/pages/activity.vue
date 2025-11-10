<template>
  <div class="space-y-8">
    <!-- Header -->
    <div>
      <h1 class="text-4xl font-bold text-white mb-2">Activity Logs</h1>
      <p class="text-gray-400">View system activity and user actions</p>
    </div>

    <!-- Filters -->
    <div class="bg-gray-900 border border-gray-800 rounded-lg p-6">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-300 mb-2">User</label>
          <select class="w-full bg-gray-800 border border-gray-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-blue-500">
            <option>All Users</option>
            <option>admin</option>
            <option>developer</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-300 mb-2">Action</label>
          <select class="w-full bg-gray-800 border border-gray-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-blue-500">
            <option>All Actions</option>
            <option>Create</option>
            <option>Update</option>
            <option>Delete</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-300 mb-2">Date Range</label>
          <input type="date" class="w-full bg-gray-800 border border-gray-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-blue-500">
        </div>
      </div>
    </div>

    <!-- Activity Timeline -->
    <div class="space-y-3">
      <div v-for="log in activityLogs" :key="log.id" class="bg-gray-900 border border-gray-800 rounded-lg p-6">
        <div class="flex items-start justify-between">
          <div class="flex gap-4 flex-1">
            <div :class="{
              'w-12 h-12 rounded-lg flex items-center justify-center flex-shrink-0': true,
              'bg-blue-600/20': log.action === 'Upload',
              'bg-purple-600/20': log.action === 'Delete',
              'bg-green-600/20': log.action === 'Create',
              'bg-yellow-600/20': log.action === 'Update'
            }">
              <i :class="{
                'fas text-lg': true,
                'fa-upload text-blue-500': log.action === 'Upload',
                'fa-trash text-purple-500': log.action === 'Delete',
                'fa-plus text-green-500': log.action === 'Create',
                'fa-edit text-yellow-500': log.action === 'Update'
              }"></i>
            </div>

            <div class="flex-1">
              <div class="flex items-center gap-2 mb-1">
                <p class="text-white font-medium">{{ log.user }}</p>
                <span class="text-gray-500 text-sm">{{ log.action }}</span>
              </div>
              <p class="text-gray-400 text-sm mb-2">{{ log.description }}</p>
              <div class="flex items-center gap-4 text-xs text-gray-500">
                <span><i class="fas fa-box mr-1"></i>{{ log.resource }}</span>
                <span><i class="fas fa-clock mr-1"></i>{{ formatTime(log.timestamp) }}</span>
              </div>
            </div>
          </div>

          <span :class="{
            'px-3 py-1 rounded-full text-sm font-medium flex-shrink-0': true,
            'bg-green-600/20 text-green-400': log.status === 'Success',
            'bg-red-600/20 text-red-400': log.status === 'Failed'
          }">
            {{ log.status }}
          </span>
        </div>
      </div>
    </div>

    <!-- Pagination -->
    <div class="flex items-center justify-between">
      <p class="text-gray-400 text-sm">Showing 1-10 of {{ activityLogs.length }} logs</p>
      <div class="flex gap-2">
        <button class="px-4 py-2 bg-gray-800 hover:bg-gray-700 text-gray-300 rounded-lg transition-colors">Previous</button>
        <button class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors">Next</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

interface ActivityLog {
  id: string
  user: string
  action: 'Create' | 'Update' | 'Delete' | 'Upload'
  description: string
  resource: string
  timestamp: Date
  status: 'Success' | 'Failed'
}

const activityLogs = ref<ActivityLog[]>([
  {
    id: '1',
    user: 'admin',
    action: 'Upload',
    description: 'Image pushed to repository',
    resource: 'nginx:latest',
    timestamp: new Date(Date.now() - 300000),
    status: 'Success'
  },
  {
    id: '2',
    user: 'developer',
    action: 'Create',
    description: 'New repository created',
    resource: 'myapp',
    timestamp: new Date(Date.now() - 3600000),
    status: 'Success'
  },
  {
    id: '3',
    user: 'admin',
    action: 'Delete',
    description: 'Tag deleted from repository',
    resource: 'redis:5.0',
    timestamp: new Date(Date.now() - 7200000),
    status: 'Success'
  },
  {
    id: '4',
    user: 'viewer',
    action: 'Update',
    description: 'Repository visibility changed',
    resource: 'postgres',
    timestamp: new Date(Date.now() - 10800000),
    status: 'Failed'
  },
  {
    id: '5',
    user: 'developer',
    action: 'Upload',
    description: 'Image pushed to repository',
    resource: 'myapp:v2.0.0',
    timestamp: new Date(Date.now() - 86400000),
    status: 'Success'
  }
])

const formatTime = (date: Date) => {
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const minutes = Math.floor(diff / 60000)
  const hours = Math.floor(diff / 3600000)
  const days = Math.floor(diff / 86400000)

  if (minutes < 60) return `${minutes}m ago`
  if (hours < 24) return `${hours}h ago`
  return `${days}d ago`
}
</script>
