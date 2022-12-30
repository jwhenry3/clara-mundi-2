import { config } from '@app/core'
import { NestFactory } from '@nestjs/core'

import { ItemServerModule } from './item-server.module'

async function bootstrap() {
  const app = await NestFactory.create(ItemServerModule.forRoot())
  await app.listen(config.item.httpPort)
}
bootstrap()
