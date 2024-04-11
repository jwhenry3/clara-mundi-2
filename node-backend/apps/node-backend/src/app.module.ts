import { AuthModule } from '@app/auth'
import { DatabaseModule } from '@app/database'
import { Module } from '@nestjs/common'
import { LoginServerModule } from 'apps/login-server/src/login-server.module'
import { MasterServerModule } from 'apps/master-server/src/master-server.module'

import { AppController } from './app.controller'
import { AppService } from './app.service'
require('dotenv').config()

console.log(process.env.TYPEORM_PORT)
@Module({
  imports: [AuthModule, MasterServerModule, LoginServerModule, DatabaseModule.forRoot(process.env)],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
