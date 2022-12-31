import { config, CoreUtils } from '@app/core'
import { NestFactory } from '@nestjs/core'

import { PartyServerModule } from './party-server.module'

async function bootstrap() {
  const app = await NestFactory.create(PartyServerModule.forRoot())
  await CoreUtils.connectRedis(app)
  await app.listen(config.party.httpPort)
}
bootstrap()
