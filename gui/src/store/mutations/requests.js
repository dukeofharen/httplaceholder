export function storeRequests(state, requests) {
    state.requests = requests;
}

export function addAdditionalRequest(state, request) {
    state.requests.unshift(request);
}