<template>
  <accordion-item>
    <template v-slot:button-text>
      <span>{{ result.stubId }}</span>
      <span>&nbsp;</span>
      <span>(</span>
      <span
        class="fw-bold"
        :class="{
          'text-success': result.passed,
          'text-danger': !result.passed,
        }"
        >{{ result.passed ? $translate('request.passed') : $translate('request.notPassed') }}</span
      >
      <span>)</span>
    </template>
    <template v-slot:accordion-body>
      <span
        ><button class="btn btn-success btn-sm" @click="goToStub">
          {{ $translate('request.goToStub') }}
        </button></span
      >
      <div v-if="!result.conditions.length">
        {{ $translate('request.noConditionCheckersFound') }}
      </div>
      <div v-else v-for="condition of result.conditions" :key="condition.checkerName">
        <label class="fw-bold">{{ condition.checkerName }}</label>
        <div
          :class="{
            'text-success': condition.conditionValidation === ConditionValidationType.Valid,
            'text-danger': condition.conditionValidation === ConditionValidationType.Invalid,
          }"
        >
          {{
            condition.conditionValidation === ConditionValidationType.Valid
              ? $translate('request.passed')
              : $translate('request.notPassed')
          }}
        </div>
        <div v-if="condition.log">{{ condition.log }}</div>
        <hr />
      </div>
    </template>
  </accordion-item>
</template>

<script lang="ts">
import { defineComponent, type PropType } from 'vue'
import type { StubExecutionResultModel } from '@/domain/request/stub-execution-result-model'
import { ConditionValidationType } from '@/domain/request/enums/condition-validation-type'
import { useRouter } from 'vue-router'

export default defineComponent({
  name: 'StubExecutionResult',
  props: {
    correlationId: {
      type: String,
      required: true,
    },
    result: {
      type: Object as PropType<StubExecutionResultModel>,
      required: true,
    },
  },
  setup(props) {
    const router = useRouter()

    // Methods
    const goToStub = async () => {
      await router.push({
        name: 'Stubs',
        query: { filter: props.result.stubId },
      })
    }
    return { ConditionValidationType, goToStub }
  },
})
</script>

<style scoped>
label {
  display: block;
}
</style>
