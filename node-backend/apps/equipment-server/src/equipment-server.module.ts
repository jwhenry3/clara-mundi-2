import { AuthModule } from '@app/auth'
import { CoreUtils } from '@app/core'
import { DatabaseModule } from '@app/database'
import { Module } from '@nestjs/common'

import { EquipmentServerController } from './equipment-server.controller'
import { EquipmentServerService } from './equipment-server.service'

@Module({
  imports: [AuthModule, CoreUtils.importClient('EQUIPMENT_SERVICE')],
  controllers: [EquipmentServerController],
  providers: [EquipmentServerService],
})
export class EquipmentServerModule {
  static forRoot() {
    return {
      module: EquipmentServerModule,
      imports: [DatabaseModule],
    }
  }
}
