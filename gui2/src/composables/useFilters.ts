import { useSettingsStore } from '@/stores/settings.ts'
import type { SavedFilterModel } from '@/domain/ui/saved-filter-model.ts'
import { getRequestFilterForm, setRequestFilterForm } from '@/utils/session.ts'
import { computed, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import type { RequestOverviewModel } from '@/domain/request/request-overview-model.ts'

/**
 * A simple composable for keeping track of requests / stubs and filtering them.
 */
export function useFilters() {
  const route = useRoute()
  const settingsStore = useSettingsStore()
  const saveSearchFilters = settingsStore.getSaveSearchFilters
  let savedFilter: SavedFilterModel = {
    filter: '',
    selectedTenantName: '',
  }
  if (saveSearchFilters) {
    savedFilter = getRequestFilterForm()
  }

  // Data
  const requests = ref<RequestOverviewModel[]>([])
  const filter = ref<SavedFilterModel>({
    filter: (route.query.filter as string) || savedFilter?.filter || '',
    selectedTenantName: (route.query.tenant as string) || savedFilter?.selectedTenantName || '',
  })

  // Computed
  const filteredRequests = computed(() => {
    let result = requests.value
    if (filter.value.filter) {
      const searchTerm = filter.value.filter.toLowerCase().trim()
      result = result.filter((r) => {
        const stubId = r.executingStubId ? r.executingStubId.toLowerCase() : ''
        const url = r.url.toLowerCase()
        const correlationId = (r.correlationId || '').toLowerCase()
        return (
          (stubId && stubId.includes(searchTerm)) ||
          url.includes(searchTerm) ||
          correlationId === searchTerm
        )
      })
    }

    if (filter.value.selectedTenantName) {
      result = result.filter((r) => r.stubTenant === filter.value.selectedTenantName)
    }

    return result
  })

  // Functions
  const filterChanged = () => {
    if (settingsStore.getSaveSearchFilters) {
      setRequestFilterForm(filter.value)
    }
  }

  // Watch
  watch(filter, () => filterChanged(), { deep: true })

  return { filteredRequests, requests, filter }
}
