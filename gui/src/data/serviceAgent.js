import axios from 'axios'
import urls from 'urls'

const getRequests = (username, password) => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/requests`
    let config = getConfig(username, password)
    return axios.get(url, config)
}

const getStubs = (username, password) => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/stubs`
    let config = getConfig(username, password)
    return axios.get(url, config)
}

const deleteAllRequests = (username, password) => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/requests`
    let config = getConfig(username, password)
    return axios.delete(url, config)
}

const getUser = (username, password) => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/users/${username}`
    let config = getConfig(username, password)
    return axios.get(url, config)
}

const getConfig = (username, password) => {
    let basicAuthString = btoa(`${username}:${password}`)
    return {
        headers: {
            Authorization: `Basic ${basicAuthString}`
        }
    }
}

export {
    getRequests,
    getStubs,
    deleteAllRequests,
    getUser
}