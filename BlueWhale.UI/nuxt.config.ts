// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  ssr: false,
  srcDir: '.',
  
  modules: [
    '@nuxtjs/tailwindcss',
    '@pinia/nuxt'
  ],
  
  runtimeConfig: {
    public: {
      apiBase: process.env.API_BASE || 'http://localhost:5260/v1/api',
      registryUrl: process.env.REGISTRY_URL || 'http://localhost:5000'
    }
  },
  
  css: [
    './assets/css/globals.css'
  ],
  
  app: {
    head: {
      title: 'BlueWhale - Docker Registry Manager',
      meta: [
        { charset: 'utf-8' },
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
    }
  }
})
