import axios from 'axios'
import urls from 'urls'

export default {
    refreshMetadata({ commit }) {
        let rootUrl = urls.rootUrl
        let url = `${rootUrl}ph-api/metadata`
        axios
          .get(url)
          .then(response => {
              commit('storeMetadata', response.data)
          })
    }
}