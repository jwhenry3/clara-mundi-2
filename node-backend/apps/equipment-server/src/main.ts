import { config } from '@app/core'
import { NestFactory } from '@nestjs/core'

import { EquipmentServerModule } from './equipment-server.module'

async function bootstrap() {
  const app = await NestFactory.create(EquipmentServerModule.forRoot())
  await app.listen(config.equipment.httpPort)
}
bootstrap()
