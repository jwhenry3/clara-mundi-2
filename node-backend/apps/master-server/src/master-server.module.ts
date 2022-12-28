import { AuthModule } from '@app/auth'
import { DatabaseModule } from '@app/database'
import { Module } from '@nestjs/common'

import { MasterServerController } from './master-server.controller'
import { MasterServerGateway } from './master-server.gateway'
import { MasterServerService } from './master-server.service'

@Module({
  imports: [AuthModule],
  controllers: [MasterServerController],
  providers: [MasterServerService, MasterServerGateway, AuthModule],
})
export class MasterServerModule {
  static forRoot() {
    return {
      module: MasterServerModule,
      imports: [DatabaseModule],
    }
  }
}
