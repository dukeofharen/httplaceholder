import axios from 'axios'
import urls from 'urls'

const getRequests = () => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/requests`
    return axios.get(url)
}

export {
    getRequests
}