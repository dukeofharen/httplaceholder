import { onMounted, onUnmounted } from 'vue'

/**
 * A simple composable for listening to specific key presses globally.
 * Registers the listener when the component is mounted and de-registers it when the component is unmounted.
 * @param key the keyboard key to listen for.
 * @param callback the callback to execute when the key is pressed.
 */
export function useGlobalKeyPress(key: string, callback: () => void | Promise<void>) {
  // Functions
  function handleKeyboardEvent(e: KeyboardEvent) {
    if (e.key === key && callback) {
      callback()
    }
  }

  function registerEvent() {
    document.addEventListener('keyup', handleKeyboardEvent)
  }

  function unregisterEvent() {
    document.removeEventListener('keyup', handleKeyboardEvent)
  }

  // Lifecycle
  onMounted(() => registerEvent())
  onUnmounted(() => unregisterEvent())
}
