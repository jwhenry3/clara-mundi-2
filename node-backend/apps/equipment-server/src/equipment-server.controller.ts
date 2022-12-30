import { Controller, Get } from '@nestjs/common'

import { EquipmentServerService } from './equipment-server.service'

@Controller('equipment-server')
export class EquipmentServerController {
  constructor(
    private readonly equipmentServerService: EquipmentServerService,
  ) {}

  @Get()
  getHello(): string {
    return this.equipmentServerService.getHello()
  }
}
