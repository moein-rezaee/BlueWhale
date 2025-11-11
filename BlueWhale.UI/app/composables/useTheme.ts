import { ref, watch, onMounted } from 'vue'

export type Theme = 'light' | 'dark'

const theme = ref<Theme>('dark')
const isInitialized = ref(false)

export const useTheme = () => {
  const setTheme = (newTheme: Theme) => {
    theme.value = newTheme
    
    if (typeof document !== 'undefined') {
      if (newTheme === 'dark') {
        document.documentElement.classList.add('dark')
      } else {
        document.documentElement.classList.remove('dark')
      }
      
      // Save to localStorage
      localStorage.setItem('theme', newTheme)
    }
  }

  const toggleTheme = () => {
    setTheme(theme.value === 'dark' ? 'light' : 'dark')
  }

  const initTheme = () => {
    if (isInitialized.value) return
    
    if (typeof window !== 'undefined') {
      // Load from localStorage or use system preference
      const savedTheme = localStorage.getItem('theme') as Theme | null
      
      if (savedTheme) {
        setTheme(savedTheme)
      } else {
        // Check system preference
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches
        setTheme(prefersDark ? 'dark' : 'light')
      }
      
      isInitialized.value = true
    }
  }

  return {
    theme,
    setTheme,
    toggleTheme,
    initTheme
  }
}
