import { useState } from 'react'

export function Auth() {
  const [isRegister, setIsRegister] = useState(false)

  return (
    <view className="bg-slate-800 w-64 text-white flex flex-col gap-4 p-2">
      <text className="text-center">Clara Mundi</text>
      <input placeholder="Username" className="h-8 p-2" />
      <input placeholder="Password" className="h-8 p-2" />
      {isRegister && (
        <input placeholder="Confirm Password" className="h-8 p-2" />
      )}
      <view className="flex flex-row gap-4 justify-around">
        <button className="p-2 px-4 bg-slate-700">
          <text>{isRegister ? 'Register' : 'Login'}</text>
        </button>
        <button className="p-2 px-4" onClick={() => setIsRegister(!isRegister)}>
          <text>{isRegister ? 'Have Account?' : 'No Account?'}</text>
        </button>
      </view>
    </view>
  )
}
