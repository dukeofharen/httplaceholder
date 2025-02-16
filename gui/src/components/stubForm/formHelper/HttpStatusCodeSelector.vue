<template>
  <div class="list-group">
    <button
      v-for="(code, index) of httpStatusCodes"
      :key="index"
      class="list-group-item list-group-item-action fw-bold"
      @click="codeSelected(code)"
    >
      {{ code.code }} - {{ code.name }}
    </button>
  </div>
</template>

<script lang="ts">
import { useStubFormStore } from '@/store/stubForm'
import { defineComponent } from 'vue'
import { type HttpStatusCode, httpStatusCodes } from '@/domain/stubForm/http-status-codes'

export default defineComponent({
  name: 'HttpStatusCodeSelector',
  setup() {
    const stubFormStore = useStubFormStore()

    // Methods
    const codeSelected = (code: HttpStatusCode) => {
      stubFormStore.setStatusCode(code.code)
      stubFormStore.closeFormHelper()
    }

    return { httpStatusCodes, codeSelected }
  },
})
</script>

<style scoped></style>
