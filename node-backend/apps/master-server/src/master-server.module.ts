import { AuthModule } from '@app/auth'
import { Module } from '@nestjs/common'

import { MasterServerController } from './master-server.controller'
import { MasterServerGateway } from './master-server.gateway'
import { MasterServerService } from './master-server.service'

@Module({
  imports: [AuthModule],
  controllers: [MasterServerController],
  providers: [MasterServerService, MasterServerGateway],
})
export class MasterServerModule {}
