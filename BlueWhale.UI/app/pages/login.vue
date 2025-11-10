<template>
  <div class="min-h-screen bg-gradient-to-br from-gray-900 via-gray-800 to-gray-900 flex items-center justify-center px-4">
    <!-- Login Card -->
    <div class="w-full max-w-md">
      <!-- Logo & Title -->
      <div class="text-center mb-8">
        <div class="inline-flex items-center justify-center w-16 h-16 bg-gradient-to-br from-blue-500 to-cyan-500 rounded-xl mb-4">
          <i class="fas fa-whale text-white text-2xl"></i>
        </div>
        <h1 class="text-4xl font-bold text-white mb-2">BlueWhale</h1>
        <p class="text-gray-400">Docker Registry Management Panel</p>
      </div>

      <!-- Login Form Card -->
      <div class="bg-gray-800/80 backdrop-blur border border-gray-700 rounded-lg shadow-xl p-8 mb-6">
        <!-- Error Message -->
        <div v-if="error" class="mb-4 p-3 bg-red-500/10 border border-red-500 text-red-400 rounded-lg text-sm flex items-center gap-2">
          <i class="fas fa-exclamation-circle"></i>
          {{ error }}
        </div>

        <!-- Form -->
        <form @submit.prevent="handleLogin" class="space-y-4">
          <!-- Username Input -->
          <div>
            <label class="block text-gray-300 text-sm font-medium mb-2">
              <i class="fas fa-user text-blue-400 mr-2"></i>Username
            </label>
            <input
              v-model="form.username"
              type="text"
              placeholder="admin"
              class="w-full px-4 py-2 bg-gray-700 text-white border border-gray-600 rounded-lg focus:outline-none focus:border-blue-500 transition"
              :disabled="loading"
              required
            />
          </div>

          <!-- Password Input -->
          <div>
            <label class="block text-gray-300 text-sm font-medium mb-2">
              <i class="fas fa-lock text-blue-400 mr-2"></i>Password
            </label>
            <input
              v-model="form.password"
              type="password"
              placeholder="••••••••"
              class="w-full px-4 py-2 bg-gray-700 text-white border border-gray-600 rounded-lg focus:outline-none focus:border-blue-500 transition"
              :disabled="loading"
              required
            />
          </div>

          <!-- Remember Me -->
          <div class="flex items-center gap-2">
            <input
              v-model="form.rememberMe"
              type="checkbox"
              id="remember"
              class="w-4 h-4 rounded border-gray-600 bg-gray-700 text-blue-500 cursor-pointer"
              :disabled="loading"
            />
            <label for="remember" class="text-sm text-gray-400 cursor-pointer hover:text-gray-300">
              Remember me
            </label>
          </div>

          <!-- Submit Button -->
          <button
            type="submit"
            class="w-full mt-6 px-4 py-2 bg-gradient-to-r from-blue-600 to-cyan-600 text-white font-medium rounded-lg hover:from-blue-700 hover:to-cyan-700 transition disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
            :disabled="loading"
          >
            <i v-if="!loading" class="fas fa-sign-in-alt"></i>
            <i v-else class="fas fa-spinner animate-spin"></i>
            {{ loading ? 'Signing in...' : 'Sign In' }}
          </button>
        </form>
      </div>

      <!-- Demo Credentials -->
      <div class="bg-blue-500/10 border border-blue-500/50 rounded-lg p-4 text-center">
        <p class="text-gray-400 text-sm mb-2">
          <i class="fas fa-info-circle text-blue-400 mr-2"></i>Demo Credentials
        </p>
        <p class="text-gray-300 text-sm font-mono">
          <span class="text-blue-400">admin</span> / <span class="text-blue-400">admin123</span>
        </p>
      </div>

      <!-- Footer -->
      <div class="mt-8 text-center text-gray-500 text-xs">
        <p>🐳 BlueWhale Docker Registry Management</p>
        <p class="mt-1">Powered by .NET & Nuxt</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useRuntimeConfig } from '#app'

const router = useRouter()
const config = useRuntimeConfig()

const form = ref({
  username: '',
  password: '',
  rememberMe: false
})

const loading = ref(false)
const error = ref('')

const handleLogin = async () => {
  if (!form.value.username || !form.value.password) {
    error.value = 'Please fill in all fields'
    return
  }

  loading.value = true
  error.value = ''

  try {
    const response = await $fetch(`${config.public.apiBase}/Auth/Login`, {
      method: 'POST',
      body: {
        username: form.value.username,
        password: form.value.password
      }
    })

    if (response.accessToken) {
      // Store tokens
      localStorage.setItem('accessToken', response.accessToken)
      localStorage.setItem('refreshToken', response.refreshToken)
      
      if (form.value.rememberMe) {
        localStorage.setItem('rememberMe', 'true')
        localStorage.setItem('lastUsername', form.value.username)
      }

      // Store user info
      if (response.user) {
        localStorage.setItem('user', JSON.stringify(response.user))
      }

      // Redirect to dashboard
      await router.push('/dashboard')
    }
  } catch (err: any) {
    error.value = err.data?.error || err.message || 'Login failed. Please try again.'
    console.error('Login error:', err)
  } finally {
    loading.value = false
  }
}

// Check if user is already logged in
const checkAuth = () => {
  const token = localStorage.getItem('accessToken')
  if (token) {
    router.push('/dashboard')
  }
}

// Restore previous username if remembered
const restoreRemembered = () => {
  const rememberMe = localStorage.getItem('rememberMe') === 'true'
  if (rememberMe) {
    const username = localStorage.getItem('lastUsername')
    if (username) {
      form.value.username = username
      form.value.rememberMe = true
    }
  }
}

onMounted(() => {
  checkAuth()
  restoreRemembered()
})
</script>

<style scoped>
input:disabled {
  opacity: 0.6;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

:deep(.fade-enter-active) {
  animation: fadeIn 0.3s ease-out;
}
</style>
