import { ReactUnity, UnityEngine, useGlobals } from '@reactunity/renderer'
import { useCallback, useEffect, useRef, useState } from 'react'
import { useAuth } from 'src/state/auth.state'
import useKeyFocus from 'src/hooks/useKeyFocus'
import useAutoFocus from 'src/hooks/useAutoFocus'

declare type Input = ReactUnity.UGUI.InputComponent
declare type Button = ReactUnity.UGUI.ButtonComponent

export function Auth() {
  const globals = useGlobals()
  const username = useRef<Input>()
  const password = useRef<Input>()
  const confirmPassword = useRef<Input>()
  const submit = useRef<Button>()
  const changeRegister = useRef<Button>()
  const [isRegister, setIsRegister] = useState(false)
  const [error, setError] = useState('')
  const api = globals.UI.Api
  const { setToken } = useAuth()

  const onSubmit = useCallback(async () => {
    if (!isRegister) {
      if (!username.current.Value || !password.current.Value) {
        setError('Provide Username and Password')
        return
      }
    } else {
      if (
        !username.current.Value ||
        !password.current.Value ||
        !confirmPassword.current.Value
      ) {
        setError('Provide Username and Password')
        return
      } else if (confirmPassword.current.Value !== password.current.Value) {
        setError('Password and Confirm Password must match')
        return
      }
    }
  }, [api, setToken, isRegister, setError])
  const onRegisterChange = useCallback(
    (value: boolean) => {
      setError('')
      username.current.Value = ''
      password.current.Value = ''
      if (confirmPassword.current) confirmPassword.current.Value = ''
      setIsRegister(value)
    },
    [setIsRegister, setError],
  )

  const { focused, onKeyDown } = useKeyFocus('username')

  const onUsername = useAutoFocus(username)
  return (
    <view className="bg-slate-800 w-72 text-white flex flex-col gap-4 p-4">
      <text className="text-center">Clara Mundi</text>
      <input
        ref={onUsername}
        name="username"
        placeholder="Username"
        className="h-8 p-2 bg-slate-600"
        onChange={() => setError('')}
        onKeyDown={onKeyDown(changeRegister, password)}
      />
      <input
        ref={password}
        name="password"
        placeholder="Password"
        className="h-8 p-2 bg-slate-600"
        onChange={() => setError('')}
        onKeyDown={
          isRegister
            ? onKeyDown(username, confirmPassword)
            : onKeyDown(username, submit)
        }
      />
      {isRegister && (
        <input
          name="confirmPassword"
          ref={confirmPassword}
          placeholder="Confirm Password"
          className="h-8 p-2 bg-slate-600"
          onChange={() => setError('')}
          onKeyDown={onKeyDown(password, submit)}
        />
      )}
      <text className="text-center text-red-400">{error}</text>
      <view className="flex flex-row gap-4 justify-around">
        <button
          ref={submit}
          name="submit"
          className={
            'p-2 px-4 ' +
            (focused == 'submit' ? 'bg-slate-600' : 'bg-slate-700')
          }
          onClick={onSubmit}
          onKeyDown={onKeyDown(
            isRegister ? confirmPassword : password,
            changeRegister,
          )}
        >
          <text>{isRegister ? 'Register' : 'Login'}</text>
        </button>
        <button
          name="changeRegister"
          ref={changeRegister}
          className={
            'p-2 px-4 ' + (focused == 'changeRegister' ? 'bg-slate-600' : '')
          }
          onClick={() => onRegisterChange(!isRegister)}
          onKeyDown={onKeyDown(submit, username)}
        >
          <text>{isRegister ? 'Have Account?' : 'No Account?'}</text>
        </button>
      </view>
    </view>
  )
}
