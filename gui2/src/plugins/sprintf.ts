import type { App } from 'vue'
import { vsprintf } from 'sprintf-js'

export default {
  install: (app: App) => {
    app.config.globalProperties.$vsprintf = vsprintf
  },
}
