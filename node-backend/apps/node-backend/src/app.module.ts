import { Module } from '@nestjs/common'

import { ChatServerModule } from '../../chat-server/src/chat-server.module'
import { LoginServerModule } from '../../login-server/src/login-server.module'
import { MasterServerModule } from '../../master-server/src/master-server.module'
import { QuestServerModule } from '../../quest-server/src/quest-server.module'
import { AppController } from './app.controller'
import { AppService } from './app.service'

@Module({
  imports: [
    MasterServerModule,
    ChatServerModule,
    QuestServerModule,
    LoginServerModule,
  ],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
