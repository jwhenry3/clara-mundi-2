import { Injectable } from '@nestjs/common'

@Injectable()
export class AuthService {
  async loginClient(email: string, password: string) {
    return true
  }
  async registerClient(email: string, password: string) {
    return true
  }
  async validateClient(token: string) {
    return true
  }

  async validateServer(token: string) {
    return true
  }
}
