export function addAdditionalRequest(state, request) {
    state.requests.unshift(request);
}