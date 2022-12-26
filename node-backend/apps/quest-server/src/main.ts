import { NestFactory } from '@nestjs/core'
import { WsAdapter } from '@nestjs/platform-ws'

import { QuestServerModule } from './quest-server.module'

async function bootstrap() {
  const app = await NestFactory.create(QuestServerModule)
  app.useWebSocketAdapter(new WsAdapter(app))
  await app.listen(3000)
}
bootstrap()
