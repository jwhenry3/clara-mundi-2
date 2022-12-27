import { AuthModule } from '@app/auth'
import { Module } from '@nestjs/common'

import { LoginServerController } from './login-server.controller'
import { LoginServerService } from './login-server.service'

@Module({
  imports: [AuthModule],
  controllers: [LoginServerController],
  providers: [LoginServerService],
})
export class LoginServerModule {}
