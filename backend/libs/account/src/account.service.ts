import { Injectable } from '@nestjs/common'

@Injectable()
export class AccountService {
  public getAccount(accountName: string) {}
  public createAccount(accountName: string, password: string) {}
  public authorizeAccount(accountName: string, password: string) {}
}
