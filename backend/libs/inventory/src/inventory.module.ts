import { AuthModule } from '@app/auth'
import { CharacterModule } from '@app/character'
import { Module } from '@nestjs/common'

import { HasItemsGuard } from './has-items/has-items.guard'
import { InventoryService } from './inventory.service'
import { InventoryController } from './inventory/inventory.controller'

@Module({
  imports: [AuthModule, CharacterModule],
  providers: [InventoryService, HasItemsGuard],
  exports: [InventoryService, HasItemsGuard],
  controllers: [InventoryController],
})
export class InventoryModule {}
