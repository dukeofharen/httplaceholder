import { Modal } from 'bootstrap'

export function getOrCreateInstance(element: HTMLElement): Modal {
  return Modal.getOrCreateInstance(element)
}
