import { Injectable } from '@nestjs/common'
import { InjectRepository } from '@nestjs/typeorm'
import * as bcrypt from 'bcrypt'
import * as jwt from 'jsonwebtoken'
import { Repository } from 'typeorm'

import { AccountEntity } from './entities/account.entity'

export interface AuthResponse {
  account: { accountId: string; token: string }
  status: boolean
  reason: string
}
@Injectable()
export class AuthService {
  constructor(
    @InjectRepository(AccountEntity) private repo: Repository<AccountEntity>,
  ) {}
  async loginClient(email: string, password: string): Promise<AuthResponse> {
    const account = await this.repo.findOneBy({ email })
    if (!account)
      return {
        account: null,
        status: false,
        reason: 'not-found',
      }
    if (await bcrypt.compare(password, account.password))
      return {
        account: null,
        status: false,
        reason: 'password',
      }
    account.lastLogin = new Date().valueOf()
    account.token = jwt.sign(
      { email, accountId: account.accountId },
      process.env.JWT_SECRET,
    )
    await this.repo.save(account)
    return {
      account: { accountId: account.accountId, token: account.token },
      status: true,
      reason: '',
    }
  }
  async registerClient(email: string, password: string): Promise<AuthResponse> {
    let account = await this.repo.findOneBy({ email })
    if (account)
      return {
        account: null,
        status: false,
        reason: 'conflict',
      }
    account = this.repo.create({
      email,
      password: await bcrypt.hash(password, 10),
    })
    account.token = jwt.sign(
      { email, accountId: account.accountId },
      process.env.JWT_SECRET,
    )
    account.lastLogin = new Date().valueOf()
    await this.repo.save(account)
    return {
      account: { accountId: account.accountId, token: account.token },
      status: true,
      reason: '',
    }
  }
  async validateClient(token: string) {
    return true
  }

  async validateServer(token: string) {
    return true
  }
}
