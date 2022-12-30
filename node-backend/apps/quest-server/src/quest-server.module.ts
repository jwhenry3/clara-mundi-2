import { AuthModule } from '@app/auth'
import { CoreUtils } from '@app/core'
import { DatabaseModule } from '@app/database'
import { Module } from '@nestjs/common'

import { QuestServerController } from './quest-server.controller'
import { QuestServerService } from './quest-server.service'
import { QuestGateway } from './quest.gateway'

@Module({
  imports: [AuthModule, CoreUtils.importClient('QUEST_SERVICE')],
  controllers: [QuestServerController],
  providers: [QuestServerService, QuestGateway],
})
export class QuestServerModule {
  static forRoot() {
    return {
      module: QuestServerModule,
      imports: [DatabaseModule],
    }
  }
}
