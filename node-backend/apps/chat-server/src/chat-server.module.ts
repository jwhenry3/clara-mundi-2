import { AuthModule } from '@app/auth'
import { Module } from '@nestjs/common'

import { ChatServerController } from './chat-server.controller'
import { ChatServerService } from './chat-server.service'
import { ChatGateway } from './chat.gateway'

@Module({
  imports: [AuthModule],
  controllers: [ChatServerController],
  providers: [ChatServerService, ChatGateway],
})
export class ChatServerModule {}
