import { AccountModule } from '@app/account'
import { Module } from '@nestjs/common'

import { AuthService } from './auth.service'
import { AuthController } from './auth/auth.controller'

@Module({
  imports: [AccountModule],
  providers: [AuthService],
  exports: [AuthService],
  controllers: [AuthController],
})
export class AuthModule {}
