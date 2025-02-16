export function setSession(key: string, value: any) {
  sessionStorage.setItem(key, JSON.stringify(value))
}

export function getSession(key: string): any {
  const result = sessionStorage.getItem(key)
  if (!result) return null
  return JSON.parse(result)
}

export function removeSession(key: string) {
  return sessionStorage.removeItem(key)
}

export function setLocal(key: string, value: any) {
  localStorage.setItem(key, JSON.stringify(value))
}

export function getLocal(key: string): any {
  const result = localStorage.getItem(key)
  if (!result) return null
  return JSON.parse(result)
}

export function removeLocal(key: string) {
  return localStorage.removeItem(key)
}
