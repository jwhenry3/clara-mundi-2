import { AuthModule } from '@app/auth'
import { Module } from '@nestjs/common'

import { QuestServerController } from './quest-server.controller'
import { QuestServerService } from './quest-server.service'
import { QuestGateway } from './quest.gateway'

@Module({
  imports: [AuthModule],
  controllers: [QuestServerController],
  providers: [QuestServerService, QuestGateway],
})
export class QuestServerModule {}
