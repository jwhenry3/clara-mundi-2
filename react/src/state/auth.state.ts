import { create } from 'zustand'

export declare type AuthState = {
  token: string
  setToken(token: string): void
}
export const useAuth = create<AuthState>((set) => ({
  token: '',
  setToken(token) {
    set({ token })
  },
}))
