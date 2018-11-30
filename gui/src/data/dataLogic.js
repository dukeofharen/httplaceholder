import { getUser, getRequests, deleteAllRequests, getStubs, addStub, deleteStub } from "./serviceAgent"

const authenticate = (username, password, successCallback, errorCallback) => {
    
}

const logicGetRequests = () => {
    let username = getItem("username");
    let password = getItem("password");
    return getRequests(username, password);
}

const logicDeleteAllRequests = () => {
    let username = getItem("username");
    let password = getItem("password");
    return deleteAllRequests(username, password);
}

const logicGetStubs = asYaml => {
    if(!asYaml) {
        asYaml = false;
    }

    let username = getItem("username");
    let password = getItem("password");
    return getStubs(username, password, asYaml);
}

const logicAddStub = stub => {
    let username = getItem("username");
    let password = getItem("password");
    return addStub(stub, username, password);
}

const logicDeleteStub = stubId => {
    let username = getItem("username");
    let password = getItem("password");
    return deleteStub(stubId, username, password);
}

export {
    logicGetRequests,
    logicDeleteAllRequests,
    logicGetStubs,
    authenticate,
    logicAddStub,
    logicDeleteStub
}