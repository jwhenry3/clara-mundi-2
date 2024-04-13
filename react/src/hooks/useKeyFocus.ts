import { ReactUnity, UnityEngine } from '@reactunity/renderer'
import { debounce } from 'lodash'
import { RefObject, useCallback, useState } from 'react'
import { asButton, asInput } from 'src/utils/casts'

declare type Input = ReactUnity.UGUI.InputComponent
declare type Button = ReactUnity.UGUI.ButtonComponent
declare type GameObject = UnityEngine.GameObject
const UInput = Interop.UnityEngine.Input
const KeyCode = Interop.UnityEngine.KeyCode
export default function useKeyFocus(initialField: string) {
  const [focused, setFocused] = useState(initialField)
  const onTab = useCallback(
    debounce((input?: Input, gameObject?: GameObject) => {
      if (input) {
        input.Focus()
        setFocused(input.Name)
      }
      if (gameObject) {
        Interop.UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(
          gameObject,
        )
        setFocused(gameObject.name)
      }
    }, 100),
    [],
  )
  const onKeyDown = useCallback(
    (previous?: RefObject<Input | Button>, next?: RefObject<Input | Button>) =>
      (event: ReactUnity.UGUI.EventHandlers.KeyEventData) => {
        const shiftPressed = UInput.GetKey(KeyCode.LeftShift)
        const tabPressed = UInput.GetKey(KeyCode.Tab)
        if (shiftPressed && tabPressed) {
          const nextInput = asInput(previous)
          const nextButton = asButton(previous)
          onTab(nextInput, nextButton?.GameObject)
        } else if (tabPressed) {
          const nextInput = asInput(next)
          const nextButton = asButton(next)
          onTab(nextInput, nextButton?.GameObject)
        }
      },
    [],
  )
  return { focused, onKeyDown }
}
