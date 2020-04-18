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
