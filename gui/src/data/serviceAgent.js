import axios from 'axios'
import urls from 'urls'

const getRequests = (username, password) => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/requests`
    let config = getConfig(username, password)
    return axios.get(url, config)
}

const getStubs = (username, password, asYaml) => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/stubs`
    let config = getConfig(username, password, asYaml)
    return axios.get(url, config)
}

const deleteAllRequests = (username, password) => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/requests`
    let config = getConfig(username, password)
    return axios.delete(url, config)
}

const addStub = (stub, username, password) => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/stubs`
    let config = getConfig(username, password)
    config.headers["Content-Type"] = 'application/json'
    return axios.post(url, stub, config)
}

const deleteStub = (stubId, username, password) => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/stubs/${stubId}`
    let config = getConfig(username, password)
    return axios.delete(url, config)
}

export {
    getRequests,
    getStubs,
    deleteAllRequests,
    addStub,
    deleteStub
}