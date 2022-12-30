import { CoreUtils } from '@app/core'
import { config } from '@app/core'
import { NestFactory } from '@nestjs/core'
import { WsAdapter } from '@nestjs/platform-ws'

import { ChatServerModule } from './chat-server.module'

async function bootstrap() {
  const app = await NestFactory.create(ChatServerModule.forRoot())
  app.useWebSocketAdapter(new WsAdapter(app))
  await CoreUtils.connectRedis(app)
  await app.listen(config.chat.httpPort)
}
bootstrap()
