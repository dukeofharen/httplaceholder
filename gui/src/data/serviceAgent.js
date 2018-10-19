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

const getUser = (username, password) => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/users/${username}`
    let config = getConfig(username, password)
    return axios.get(url, config)
}

const addStub = (stub, username, password) => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/stubs`
    let config = getConfig(username, password)
    config.headers["Content-Type"] = 'application/json'
    return axios.post(url, stub, config)
}

const getConfig = (username, password, asYaml) => {
    if(!asYaml) {
        asYaml = false;
    }

    let basicAuthString = btoa(`${username}:${password}`)
    let headers = {
        Authorization: `Basic ${basicAuthString}`
    };
    if (asYaml) {
        headers['Accept'] = 'text/yaml'
    }

    return {
        headers: headers
    }
}

export {
    getRequests,
    getStubs,
    deleteAllRequests,
    getUser,
    addStub
}