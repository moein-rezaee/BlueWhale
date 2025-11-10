<template>
  <div class="bg-gray-950 text-gray-100 min-h-screen">
    <NuxtRouteAnnouncer />
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </div>
</template>

<script setup lang="ts">
useHead({
  title: 'BlueWhale Registry Panel',
  meta: [
    { name: 'viewport', content: 'width=device-width, initial-scale=1' },
    { name: 'description', content: 'Docker Registry Management Panel' }
  ],
  link: [
    {
      rel: 'stylesheet',
      href: 'https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css',
      integrity: 'sha512-DTOQO6YPLF+6pNp9K3p7MC+2zCG3tPSowZjWe0aXQOi3RAnVy16U8D+fn2UtKVYbRyIQf4XVk+ztf7aJPco+A==',
      crossorigin: 'anonymous'
    }
  ]
})
</script>

