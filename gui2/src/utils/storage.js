export function setSession(key, value) {
  sessionStorage.setItem(key, JSON.stringify(value));
}

export function getSession(key) {
  const result = sessionStorage.getItem(key);
  if (!result) return null;
  return JSON.parse(result);
}

export function removeSession(key) {
  return sessionStorage.removeItem(key);
}

export function setLocal(key, value) {
  localStorage.setItem(key, JSON.stringify(value));
}

export function getLocal(key) {
  const result = localStorage.getItem(key);
  if (!result) return null;
  return JSON.parse(result);
}

export function removeLocal(key) {
  return localStorage.removeItem(key);
}
