import { AuthModule } from '@app/auth'
import { DatabaseModule } from '@app/database'
import { Module } from '@nestjs/common'

import { CharacterServerModule } from '../../character-server/src/character-server.module'
import { ChatServerModule } from '../../chat-server/src/chat-server.module'
import { EquipmentServerModule } from '../../equipment-server/src/equipment-server.module'
import { ItemServerModule } from '../../item-server/src/item-server.module'
import { LoginServerModule } from '../../login-server/src/login-server.module'
import { MasterServerModule } from '../../master-server/src/master-server.module'
import { PartyServerModule } from '../../party-server/src/party-server.module'
import { QuestServerModule } from '../../quest-server/src/quest-server.module'
import { AppController } from './app.controller'
import { AppService } from './app.service'

@Module({
  imports: [
    AuthModule,
    MasterServerModule,
    CharacterServerModule,
    ChatServerModule,
    ItemServerModule,
    EquipmentServerModule,
    PartyServerModule,
    QuestServerModule,
    LoginServerModule,
    DatabaseModule,
  ],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
