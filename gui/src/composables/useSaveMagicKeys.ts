import { useMagicKeys, whenever } from '@vueuse/core'

export function useSaveMagicKeys() {
  function registerSaveFunction(func: () => void) {
    const { ctrl_s, meta_s, ctrl_enter, meta_enter } = useMagicKeys({
      passive: false,
      onEventFired(e: KeyboardEvent) {
        if ((e.ctrlKey || e.metaKey) && (e.key === 's' || e.key === 'Enter')) {
          e.preventDefault()
        }
      },
    })
    whenever(ctrl_s, () => func())
    whenever(ctrl_enter, () => func())
    whenever(meta_s, () => func())
    whenever(meta_enter, () => func())
  }

  return { registerSaveFunction }
}
