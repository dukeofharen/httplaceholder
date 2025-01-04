import { onMounted, onUnmounted, ref } from 'vue'
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr'
import { getRootUrl } from '@/utils/config.ts'

/**
 * Simple composable for working with SignalR connections.
 * @param relativeUrl the relative URL of the SignalR hub (e.g. '/requestHub').
 * @param methodName the method name of SignalR (e.g. 'RequestReceived').
 * @param handlingMethod the method that will handle the incoming SignalR message.
 */
export function useSignalR(
  relativeUrl: string,
  methodName: string,
  handlingMethod: (...args: any[]) => any,
) {
  // Data
  const signalrConnection = ref<HubConnection | undefined>()

  // Functions
  async function initSignalR() {
    signalrConnection.value = new HubConnectionBuilder()
      .withUrl(`${getRootUrl()}${relativeUrl}`)
      .build()
    signalrConnection.value.on(methodName, handlingMethod)
    try {
      await signalrConnection.value.start()
    } catch (err: any) {
      console.log(err.toString())
    }
  }

  // Lifecycle
  onMounted(async () => await initSignalR())
  onUnmounted(async () => {
    if (signalrConnection.value) {
      await signalrConnection.value.stop()
    }
  })
}
