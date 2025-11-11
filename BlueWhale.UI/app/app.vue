<template>
  <div class="bg-gray-950 text-gray-100 min-h-screen flex flex-col">
    <NuxtRouteAnnouncer />
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useTheme } from '~/composables/useTheme'

const { initTheme } = useTheme()

useHead({
  title: 'BlueWhale - Docker Registry Manager',
  meta: [
    { name: 'viewport', content: 'width=device-width, initial-scale=1' },
    { name: 'description', content: 'Professional Docker Registry Management Panel' },
    { name: 'theme-color', content: '#1f2937' }
  ],
  link: [
    {
      rel: 'stylesheet',
      href: 'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.7.0/css/all.min.css',
      integrity: 'sha512-9usAa3IW2lsEYNgXniLObEaJUw7Lw+ibzaqxlgkjigzoYnbaQQD60nAIIqCqJz2k/8GsLcYpVIkUfdpdObvwDg==',
      crossorigin: 'anonymous',
      referrerpolicy: 'no-referrer'
    }
  ]
})

onMounted(() => {
  initTheme()
  // Ensure Font Awesome is loaded
  const link = document.querySelector('link[href*="font-awesome"]')
  if (link && !window.FontAwesome) {
    console.log('✅ Font Awesome CSS loaded successfully')
  }
})
</script>

<style scoped>
:deep(html, body) {
  @apply bg-gray-950 text-gray-100 m-0 p-0;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif;
}
</style>

