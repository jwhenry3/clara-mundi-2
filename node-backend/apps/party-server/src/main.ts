import { config, CoreUtils } from '@app/core'
import { NestFactory } from '@nestjs/core'
import { WsAdapter } from '@nestjs/platform-ws'

import { PartyServerModule } from './party-server.module'

async function bootstrap() {
  const app = await NestFactory.create(PartyServerModule.forRoot())
  app.useWebSocketAdapter(new WsAdapter(app))
  await CoreUtils.connectRedis(app)
  await app.listen(config.party.httpPort)
}
bootstrap()
