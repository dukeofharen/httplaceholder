import { getUser, getRequests, deleteAllRequests, getStubs } from "./serviceAgent"
import { getItem, setItem } from "./session"

const shouldAuthenticate = callback => {
    let username = getItem("username") || "testUser";
    let password = getItem("password") || "testPassword";
    getUser(username, password)
        .then(response => callback(false))
        .catch(error => {
            if (error.response.status === 401) {
                callback(true)
            }
        })
}

const authenticate = (username, password, successCallback, errorCallback) => {
    getUser(username, password)
        .then(response => {
            setItem("username", username);
            setItem("password", password);
            successCallback(response);
        })
        .catch(error => {
            errorCallback(error);
        })
}

const logicGetRequests = () => {
    let username = getItem("username");
    let password = getItem("password");
    return getRequests(username, password)
}

const logicDeleteAllRequests = () => {
    let username = getItem("username");
    let password = getItem("password");
    return deleteAllRequests(username, password)
}

const logicGetStubs = () => {
    let username = getItem("username");
    let password = getItem("password");
    return getStubs(username, password)
}

export {
    shouldAuthenticate,
    logicGetRequests,
    logicDeleteAllRequests,
    logicGetStubs,
    authenticate
}