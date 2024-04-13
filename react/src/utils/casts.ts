import { ReactUnity } from '@reactunity/renderer'
import { RefObject } from 'react'

declare type Input = ReactUnity.UGUI.InputComponent
declare type Button = ReactUnity.UGUI.ButtonComponent
export const asInput = (ref?: RefObject<Input | Button>): Input | undefined => {
  if (!ref) return undefined
  if (ref.current && 'Focus' in ref.current) return ref.current as Input
  return undefined
}
export const asButton = (
  ref?: RefObject<Input | Button>,
): Button | undefined => {
  if (!ref) return undefined
  if (ref.current && !('Focus' in ref.current)) return ref.current as Button
  return undefined
}
