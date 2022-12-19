import { Injectable } from '@nestjs/common'

@Injectable()
export class AuthService {
  public login(accountName: string, password: string) {}
  public register(accountName: string, password: string) {}
  public forgotPassword(accountName: string) {}
}
