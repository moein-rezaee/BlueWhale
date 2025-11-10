<template>
  <div class="min-h-screen bg-gray-900 p-6">
    <!-- Header -->
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-white mb-2 flex items-center gap-3">
        <i class="fas fa-history text-blue-400"></i>
        Activity Logs
      </h1>
      <p class="text-gray-400">Monitor and review all system activities and operations</p>
    </div>

    <!-- Controls -->
    <div class="bg-gray-800 rounded-lg border border-gray-700 p-4 mb-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <!-- Search -->
        <div>
          <label class="block text-sm text-gray-400 mb-2">
            <i class="fas fa-search mr-2"></i>Search Action
          </label>
          <input
            v-model="filters.action"
            type="text"
            placeholder="e.g., DELETE_REPOSITORY"
            class="w-full px-3 py-2 bg-gray-700 text-white border border-gray-600 rounded-lg focus:outline-none focus:border-blue-500 transition text-sm"
          />
        </div>

        <!-- Date Range -->
        <div>
          <label class="block text-sm text-gray-400 mb-2">
            <i class="fas fa-calendar mr-2"></i>Start Date
          </label>
          <input
            v-model="filters.startDate"
            type="date"
            class="w-full px-3 py-2 bg-gray-700 text-white border border-gray-600 rounded-lg focus:outline-none focus:border-blue-500 transition text-sm"
          />
        </div>

        <div>
          <label class="block text-sm text-gray-400 mb-2">
            <i class="fas fa-calendar mr-2"></i>End Date
          </label>
          <input
            v-model="filters.endDate"
            type="date"
            class="w-full px-3 py-2 bg-gray-700 text-white border border-gray-600 rounded-lg focus:outline-none focus:border-blue-500 transition text-sm"
          />
        </div>

        <!-- Actions -->
        <div class="flex items-end gap-2">
          <button
            @click="fetchActivities"
            class="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition flex items-center justify-center gap-2 text-sm"
            :disabled="loading"
          >
            <i class="fas fa-filter"></i>
            {{ loading ? 'Loading...' : 'Filter' }}
          </button>
          <button
            @click="resetFilters"
            class="px-4 py-2 bg-gray-700 text-white rounded-lg hover:bg-gray-600 transition flex items-center gap-2 text-sm"
          >
            <i class="fas fa-redo"></i>
          </button>
        </div>
      </div>
    </div>

    <!-- Summary Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-gray-800 rounded-lg border border-gray-700 p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-400 text-sm">Total Activities</p>
            <p class="text-2xl font-bold text-white mt-1">{{ summary?.totalActivities || 0 }}</p>
          </div>
          <i class="fas fa-list-check text-blue-400 text-3xl opacity-20"></i>
        </div>
      </div>

      <div class="bg-gray-800 rounded-lg border border-gray-700 p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-400 text-sm">Last 24 Hours</p>
            <p class="text-2xl font-bold text-white mt-1">{{ summary?.last24Hours || 0 }}</p>
          </div>
          <i class="fas fa-clock text-green-400 text-3xl opacity-20"></i>
        </div>
      </div>

      <div class="bg-gray-800 rounded-lg border border-gray-700 p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-400 text-sm">Successful</p>
            <p class="text-2xl font-bold text-white mt-1">{{ summary?.successfulOperations || 0 }}</p>
          </div>
          <i class="fas fa-check-circle text-green-400 text-3xl opacity-20"></i>
        </div>
      </div>

      <div class="bg-gray-800 rounded-lg border border-gray-700 p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-gray-400 text-sm">Failed</p>
            <p class="text-2xl font-bold text-white mt-1">{{ summary?.failedOperations || 0 }}</p>
          </div>
          <i class="fas fa-exclamation-circle text-red-400 text-3xl opacity-20"></i>
        </div>
      </div>
    </div>

    <!-- Activities Table -->
    <div class="bg-gray-800 rounded-lg border border-gray-700 overflow-hidden">
      <div v-if="loading" class="p-8 text-center">
        <i class="fas fa-spinner animate-spin text-2xl text-blue-400"></i>
        <p class="text-gray-400 mt-3">Loading activities...</p>
      </div>

      <div v-else-if="activities.length === 0" class="p-8 text-center">
        <i class="fas fa-inbox text-4xl text-gray-700 mb-3"></i>
        <p class="text-gray-400">No activities found</p>
      </div>

      <table v-else class="w-full">
        <thead class="border-b border-gray-700 bg-gray-900/50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">Time</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">Action</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">Resource</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">User</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-400 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-700">
          <tr v-for="activity in activities" :key="activity.id" class="hover:bg-gray-700/50 transition">
            <td class="px-6 py-4 whitespace-nowrap">
              <span class="text-sm text-gray-400">{{ formatTime(activity.timestamp) }}</span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-900 text-blue-200">
                <i class="fas fa-code mr-1.5"></i>
                {{ activity.action }}
              </span>
            </td>
            <td class="px-6 py-4 text-sm text-gray-300">
              {{ activity.resourceName || '-' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span class="text-sm text-gray-300 flex items-center gap-1">
                <i class="fas fa-user-circle"></i>
                {{ activity.username }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span
                class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                :class="activity.success
                  ? 'bg-green-900 text-green-200'
                  : 'bg-red-900 text-red-200'
                "
              >
                <i :class="activity.success ? 'fas fa-check-circle' : 'fas fa-times-circle'" class="mr-1.5"></i>
                {{ activity.success ? 'Success' : 'Failed' }}
              </span>
            </td>
            <td class="px-6 py-4 text-right whitespace-nowrap">
              <button
                @click="showDetails(activity)"
                class="text-blue-400 hover:text-blue-300 text-sm flex items-center gap-1 ml-auto"
              >
                <i class="fas fa-eye"></i>
                View
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Pagination -->
    <div v-if="pagination.totalPages > 1" class="mt-6 flex items-center justify-between">
      <p class="text-sm text-gray-400">
        Page {{ pagination.pageNumber }} of {{ pagination.totalPages }}
        (Total: {{ pagination.totalCount }} activities)
      </p>
      <div class="flex gap-2">
        <button
          @click="pagination.pageNumber = Math.max(1, pagination.pageNumber - 1)"
          :disabled="pagination.pageNumber === 1"
          class="px-4 py-2 bg-gray-800 text-white rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-700 transition"
        >
          <i class="fas fa-chevron-left"></i>
        </button>
        <button
          @click="pagination.pageNumber = Math.min(pagination.totalPages, pagination.pageNumber + 1)"
          :disabled="pagination.pageNumber === pagination.totalPages"
          class="px-4 py-2 bg-gray-800 text-white rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-700 transition"
        >
          <i class="fas fa-chevron-right"></i>
        </button>
      </div>
    </div>

    <!-- Details Modal -->
    <div v-if="selectedActivity" class="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
      <div class="bg-gray-800 rounded-lg border border-gray-700 max-w-md w-full p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-bold text-white">Activity Details</h3>
          <button @click="selectedActivity = null" class="text-gray-400 hover:text-white">
            <i class="fas fa-times"></i>
          </button>
        </div>

        <div class="space-y-3 text-sm">
          <div>
            <p class="text-gray-400">Action</p>
            <p class="text-white font-mono">{{ selectedActivity.action }}</p>
          </div>
          <div>
            <p class="text-gray-400">Resource</p>
            <p class="text-white">{{ selectedActivity.resourceName || '-' }}</p>
          </div>
          <div>
            <p class="text-gray-400">User</p>
            <p class="text-white">{{ selectedActivity.username }}</p>
          </div>
          <div>
            <p class="text-gray-400">IP Address</p>
            <p class="text-white font-mono">{{ selectedActivity.ipAddress || '-' }}</p>
          </div>
          <div>
            <p class="text-gray-400">Timestamp</p>
            <p class="text-white">{{ formatDetailTime(selectedActivity.timestamp) }}</p>
          </div>
          <div>
            <p class="text-gray-400">Status</p>
            <p :class="selectedActivity.success ? 'text-green-400' : 'text-red-400'">
              {{ selectedActivity.success ? '✓ Success' : '✗ Failed' }}
            </p>
          </div>
          <div v-if="selectedActivity.details">
            <p class="text-gray-400">Details</p>
            <p class="text-white bg-gray-900 p-2 rounded text-xs break-words">{{ selectedActivity.details }}</p>
          </div>
        </div>

        <div class="mt-4 flex gap-2">
          <button
            @click="deleteActivity(selectedActivity.id)"
            class="flex-1 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition flex items-center justify-center gap-2"
          >
            <i class="fas fa-trash"></i>
            Delete
          </button>
          <button
            @click="selectedActivity = null"
            class="flex-1 px-4 py-2 bg-gray-700 text-white rounded-lg hover:bg-gray-600 transition"
          >
            Close
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRuntimeConfig } from '#app'

const config = useRuntimeConfig()

interface Activity {
  id: string
  action: string
  resourceName?: string
  success: boolean
  details?: string
  username: string
  ipAddress?: string
  timestamp: string
}

interface Summary {
  totalActivities: number
  last24Hours: number
  last7Days: number
  successfulOperations: number
  failedOperations: number
  topActions: Array<{ action: string; count: number }>
}

const activities = ref<Activity[]>([])
const summary = ref<Summary>()
const selectedActivity = ref<Activity | null>(null)
const loading = ref(false)

const filters = ref({
  action: '',
  startDate: '',
  endDate: ''
})

const pagination = ref({
  pageNumber: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0
})

const fetchActivities = async () => {
  loading.value = true
  try {
    const token = localStorage.getItem('accessToken')
    
    const query = new URLSearchParams({
      pageNumber: pagination.value.pageNumber.toString(),
      pageSize: pagination.value.pageSize.toString()
    })

    if (filters.value.action) query.append('action', filters.value.action)
    if (filters.value.startDate) query.append('startDate', filters.value.startDate)
    if (filters.value.endDate) query.append('endDate', filters.value.endDate)

    const response = await $fetch(`${config.public.apiBase}/Activities?${query}`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    if (response.data) {
      activities.value = response.data
      pagination.value = {
        pageNumber: response.pageNumber || 1,
        pageSize: response.pageSize || 20,
        totalCount: response.totalCount || 0,
        totalPages: response.totalPages || 0
      }
    }
  } catch (err) {
    console.error('Error fetching activities:', err)
  } finally {
    loading.value = false
  }
}

const fetchSummary = async () => {
  try {
    const token = localStorage.getItem('accessToken')
    const response = await $fetch(`${config.public.apiBase}/Activities/summary`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
    summary.value = response
  } catch (err) {
    console.error('Error fetching summary:', err)
  }
}

const resetFilters = () => {
  filters.value = { action: '', startDate: '', endDate: '' }
  pagination.value.pageNumber = 1
  fetchActivities()
}

const showDetails = (activity: Activity) => {
  selectedActivity.value = activity
}

const deleteActivity = async (id: string) => {
  if (!confirm('Are you sure you want to delete this activity?')) return

  try {
    const token = localStorage.getItem('accessToken')
    await $fetch(`${config.public.apiBase}/Activities/${id}`, {
      method: 'DELETE',
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
    selectedActivity.value = null
    await fetchActivities()
  } catch (err) {
    console.error('Error deleting activity:', err)
  }
}

const formatTime = (timestamp: string) => {
  return new Date(timestamp).toLocaleString()
}

const formatDetailTime = (timestamp: string) => {
  return new Date(timestamp).toLocaleString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit'
  })
}

onMounted(async () => {
  await fetchActivities()
  await fetchSummary()
})
</script>

<style scoped>
@keyframes spin {
  to { transform: rotate(360deg); }
}

.animate-spin {
  animation: spin 1s linear infinite;
}
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
