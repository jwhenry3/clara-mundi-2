import { Module } from '@nestjs/common'

import { AccountService } from './account.service'
import { AccountController } from './account/account.controller'

@Module({
  providers: [AccountService],
  exports: [AccountService],
  controllers: [AccountController],
})
export class AccountModule {}
