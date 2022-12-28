import { AuthModule } from '@app/auth'
import { DatabaseModule } from '@app/database'
import { Module } from '@nestjs/common'

import { LoginServerController } from './login-server.controller'
import { LoginServerService } from './login-server.service'

@Module({
  imports: [AuthModule],
  controllers: [LoginServerController],
  providers: [LoginServerService, AuthModule],
})
export class LoginServerModule {
  static forRoot() {
    return {
      module: LoginServerModule,
      imports: [DatabaseModule],
    }
  }
}
