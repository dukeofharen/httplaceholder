import { getUiRootUrl } from '@/utils/config'

export function renderDocLink(hashTag?: string) {
  let docsUrl = `${getUiRootUrl()}/docs/index.html`
  if (hashTag) {
    docsUrl += `#${hashTag}`
  }

  return docsUrl
}
