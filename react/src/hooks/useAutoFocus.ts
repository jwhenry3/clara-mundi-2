import { MutableRefObject } from 'react'

export default function useAutoFocus(ref: MutableRefObject<any>) {
  return (element) => {
    if (element != ref.current && element) {
      ref.current = element
      element.Focus()
    }
  }
}
