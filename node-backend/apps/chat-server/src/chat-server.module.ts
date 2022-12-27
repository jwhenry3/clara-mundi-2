import { AuthModule } from '@app/auth'
import { DatabaseModule } from '@app/database'
import { Module } from '@nestjs/common'

import { ChatServerController } from './chat-server.controller'
import { ChatServerService } from './chat-server.service'
import { ChatGateway } from './chat.gateway'

@Module({
  imports: [AuthModule],
  controllers: [ChatServerController],
  providers: [ChatServerService, ChatGateway],
})
export class ChatServerModule {
  static forRoot() {
    return {
      module: ChatServerModule,
      imports: [DatabaseModule],
    }
  }
}
