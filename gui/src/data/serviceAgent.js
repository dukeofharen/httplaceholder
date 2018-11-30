import axios from 'axios'
import urls from 'urls'

const addStub = (stub, username, password) => {
    let rootUrl = urls.rootUrl
    let url = `${rootUrl}ph-api/stubs`
    let config = getConfig(username, password)
    config.headers["Content-Type"] = 'application/json'
    return axios.post(url, stub, config)
}

export {
    addStub
}