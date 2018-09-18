import axios from 'axios'
import urls from 'urls'

const getRequests = () => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/requests`
    return axios.get(url)
}

const getStubs = () => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/stubs`
    return axios.get(url)
}

const deleteAllRequests = () => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/requests`
    return axios.delete(url)
}

export {
    getRequests,
    getStubs,
    deleteAllRequests
}