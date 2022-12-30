import { CoreUtils } from '@app/core'
import { config } from '@app/core'
import { NestFactory } from '@nestjs/core'

import { CharacterServerModule } from './character-server.module'

async function bootstrap() {
  const app = await NestFactory.create(CharacterServerModule.forRoot())
  await CoreUtils.connectRedis(app)
  await app.listen(config.character.httpPort)
}
bootstrap()
