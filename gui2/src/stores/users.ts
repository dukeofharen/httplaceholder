import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { clearUserToken, getUserToken, saveUserToken } from '@/utils/session'
import { toBase64 } from '@/utils/text'
import { get } from '@/utils/api'

export interface AuthenticationInput {
  username: string
  password: string
}

const token = getUserToken()
export const useUsersStore = defineStore('users', () => {
  // State
  const userToken = ref(token || '')

  // Getters
  const getAuthenticated = computed(() => !!userToken.value)
  const getUserToken = computed(() => userToken.value)

  // Actions
  async function authenticate(input: AuthenticationInput): Promise<any> {
    const username = input.username
    const password = input.password
    const token = toBase64(`${username}:${password}`)
    try {
      const response = await get(`/ph-api/users/${username}`, {
        headers: {
          Authorization: `Basic ${token}`,
        },
      })
      if (!token) {
        clearUserToken()
      } else {
        userToken.value = token
        saveUserToken(token)
      }

      return Promise.resolve(response)
    } catch (e) {
      return Promise.reject(e)
    }
  }

  function logOut() {
    clearUserToken()
    userToken.value = ''
    document.cookie = `HttPlaceholderLoggedin=;expires=${new Date(0).toUTCString()}`
  }

  return { getAuthenticated, getUserToken, authenticate, logOut }
})
