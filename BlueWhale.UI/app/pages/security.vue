<template>
  <div class="space-y-8">
    <!-- Header -->
    <div>
      <h1 class="text-4xl font-bold text-white mb-2">Access Control</h1>
      <p class="text-gray-400">Manage user permissions and access levels</p>
    </div>

    <!-- Users Section -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <div class="lg:col-span-2">
        <div class="bg-gray-900 border border-gray-800 rounded-lg overflow-hidden">
          <div class="p-6 border-b border-gray-800">
            <div class="flex items-center justify-between">
              <h2 class="text-xl font-bold text-white">Users</h2>
              <button class="bg-blue-600 hover:bg-blue-700 text-white font-medium py-1 px-3 rounded text-sm transition-colors">
                Add User
              </button>
            </div>
          </div>

          <div class="overflow-x-auto">
            <table class="w-full">
              <thead>
                <tr class="border-b border-gray-800 bg-gray-800/50">
                  <th class="px-6 py-4 text-left text-sm font-semibold text-gray-300">Username</th>
                  <th class="px-6 py-4 text-left text-sm font-semibold text-gray-300">Role</th>
                  <th class="px-6 py-4 text-left text-sm font-semibold text-gray-300">Status</th>
                  <th class="px-6 py-4 text-right text-sm font-semibold text-gray-300">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="user in users" :key="user.id" class="border-b border-gray-800 hover:bg-gray-800/50 transition-colors">
                  <td class="px-6 py-4 text-white font-medium">{{ user.username }}</td>
                  <td class="px-6 py-4">
                    <span :class="{
                      'px-3 py-1 rounded-full text-sm font-medium': true,
                      'bg-red-600/20 text-red-400': user.role === 'Admin',
                      'bg-blue-600/20 text-blue-400': user.role === 'User',
                      'bg-gray-600/20 text-gray-400': user.role === 'ReadOnly'
                    }">
                      {{ user.role }}
                    </span>
                  </td>
                  <td class="px-6 py-4">
                    <span :class="{
                      'px-3 py-1 rounded-full text-sm font-medium': true,
                      'bg-green-600/20 text-green-400': user.isActive,
                      'bg-red-600/20 text-red-400': !user.isActive
                    }">
                      {{ user.isActive ? 'Active' : 'Inactive' }}
                    </span>
                  </td>
                  <td class="px-6 py-4 text-right">
                    <div class="flex items-center justify-end gap-2">
                      <button class="p-2 hover:bg-gray-800 rounded-lg transition-colors text-gray-400 hover:text-white">
                        <i class="fas fa-edit"></i>
                      </button>
                      <button class="p-2 hover:bg-red-600/20 rounded-lg transition-colors text-gray-400 hover:text-red-400">
                        <i class="fas fa-trash"></i>
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- Stats Sidebar -->
      <div class="space-y-6">
        <div class="bg-gray-900 border border-gray-800 rounded-lg p-6">
          <p class="text-gray-400 text-sm font-medium mb-2">Total Users</p>
          <p class="text-3xl font-bold text-white">{{ users.length }}</p>
        </div>

        <div class="bg-gray-900 border border-gray-800 rounded-lg p-6">
          <p class="text-gray-400 text-sm font-medium mb-2">Administrators</p>
          <p class="text-3xl font-bold text-white">{{ users.filter(u => u.role === 'Admin').length }}</p>
        </div>

        <div class="bg-gray-900 border border-gray-800 rounded-lg p-6">
          <p class="text-gray-400 text-sm font-medium mb-2">Active Users</p>
          <p class="text-3xl font-bold text-white">{{ users.filter(u => u.isActive).length }}</p>
        </div>
      </div>
    </div>

    <!-- Repository Access -->
    <div class="bg-gray-900 border border-gray-800 rounded-lg p-6">
      <h2 class="text-xl font-bold text-white mb-4">Repository Access Matrix</h2>
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead>
            <tr class="border-b border-gray-800 bg-gray-800/50">
              <th class="px-6 py-4 text-left text-sm font-semibold text-gray-300">User</th>
              <th class="px-6 py-4 text-center text-sm font-semibold text-gray-300">nginx</th>
              <th class="px-6 py-4 text-center text-sm font-semibold text-gray-300">redis</th>
              <th class="px-6 py-4 text-center text-sm font-semibold text-gray-300">postgres</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="access in repositoryAccess" :key="access.userId" class="border-b border-gray-800">
              <td class="px-6 py-4 text-white font-medium">{{ access.username }}</td>
              <td class="px-6 py-4 text-center">
                <span :class="{
                  'px-2 py-1 rounded text-xs font-medium': true,
                  'bg-blue-600/20 text-blue-400': access.repos.nginx === 'Admin',
                  'bg-green-600/20 text-green-400': access.repos.nginx === 'Pull'
                }">
                  {{ access.repos.nginx }}
                </span>
              </td>
              <td class="px-6 py-4 text-center">
                <span :class="{
                  'px-2 py-1 rounded text-xs font-medium': true,
                  'bg-green-600/20 text-green-400': access.repos.redis === 'Pull',
                  'bg-gray-600/20 text-gray-400': access.repos.redis === 'None'
                }">
                  {{ access.repos.redis }}
                </span>
              </td>
              <td class="px-6 py-4 text-center">
                <span :class="{
                  'px-2 py-1 rounded text-xs font-medium': true,
                  'bg-purple-600/20 text-purple-400': access.repos.postgres === 'Push'
                }">
                  {{ access.repos.postgres }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

interface User {
  id: string
  username: string
  role: 'Admin' | 'User' | 'ReadOnly'
  isActive: boolean
}

interface RepositoryAccess {
  userId: string
  username: string
  repos: {
    nginx: string
    redis: string
    postgres: string
  }
}

const users = ref<User[]>([
  { id: '1', username: 'admin', role: 'Admin', isActive: true },
  { id: '2', username: 'developer', role: 'User', isActive: true },
  { id: '3', username: 'viewer', role: 'ReadOnly', isActive: true },
  { id: '4', username: 'inactive_user', role: 'User', isActive: false }
])

const repositoryAccess = ref<RepositoryAccess[]>([
  { userId: '1', username: 'admin', repos: { nginx: 'Admin', redis: 'Admin', postgres: 'Admin' } },
  { userId: '2', username: 'developer', repos: { nginx: 'Pull', redis: 'Pull', postgres: 'Push' } },
  { userId: '3', username: 'viewer', repos: { nginx: 'Pull', redis: 'None', postgres: 'Pull' } }
])
</script>
