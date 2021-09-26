import { getSession, removeSession, setSession } from "@/utils/storage";
import { sessionKeys } from "@/constants/sessionKeys";

export function getIntermediateStub() {
  return getSession(sessionKeys.intermediateStub);
}

export function setIntermediateStub(stub) {
  setSession(sessionKeys.intermediateStub, stub);
}

export function clearIntermediateStub() {
  removeSession(sessionKeys.intermediateStub);
}
