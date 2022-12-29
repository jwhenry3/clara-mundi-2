import { Controller, Get } from '@nestjs/common'

import { MasterServerGateway } from './master-server.gateway'
import { MasterServerService } from './master-server.service'

@Controller('master-server')
export class MasterServerController {
  constructor(private readonly masterServerService: MasterServerService) {}

  @Get()
  getHello(): string {
    return this.masterServerService.getHello()
  }

  @Get('servers')
  getServers() {
    console.log(MasterServerGateway.serverList)
    return Object.values(MasterServerGateway.serverList)
  }
}
