import { NestFactory } from '@nestjs/core'
import { WsAdapter } from '@nestjs/platform-ws'

import { MasterServerModule } from './master-server.module'

async function bootstrap() {
  const app = await NestFactory.create(MasterServerModule.forRoot())
  app.useWebSocketAdapter(new WsAdapter(app))
  await app.listen(3000)
}
bootstrap()
