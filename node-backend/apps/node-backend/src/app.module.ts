import { AuthModule } from '@app/auth'
import { DatabaseModule } from '@app/database'
import { Module } from '@nestjs/common'
import { LoginServerModule } from 'apps/login-server/src/login-server.module'
import { MasterServerModule } from 'apps/master-server/src/master-server.module'

import { AppController } from './app.controller'
import { AppService } from './app.service'

@Module({
  imports: [AuthModule, MasterServerModule, LoginServerModule, DatabaseModule],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
