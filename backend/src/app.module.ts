import { AccountModule } from '@app/account'
import { AuthModule } from '@app/auth'
import { CharacterModule } from '@app/character'
import { InventoryModule } from '@app/inventory'
import { QuestsModule } from '@app/quests'
import { Module } from '@nestjs/common'

import { AppController } from './app.controller'
import { AppService } from './app.service'

@Module({
  imports: [
    AccountModule,
    AuthModule,
    CharacterModule,
    InventoryModule,
    QuestsModule,
  ],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
