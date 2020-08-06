export function getMetadata(state) {
  return state.metadata;
}

export function getVariableHandlers(state) {
  if(!state.metadata) {
    return [];
  }

  return state.metadata.variableHandlers;
}
