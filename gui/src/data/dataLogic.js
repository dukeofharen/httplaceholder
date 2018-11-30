import { getUser, getRequests, deleteAllRequests, getStubs, addStub, deleteStub } from "./serviceAgent"

const logicAddStub = stub => {
    let username = getItem("username");
    let password = getItem("password");
    return addStub(stub, username, password);
}

export {
    logicAddStub
}