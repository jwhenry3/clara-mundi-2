import { CoreUtils } from '@app/core'
import { config } from '@app/core'
import { NestFactory } from '@nestjs/core'

import { QuestServerModule } from './quest-server.module'

async function bootstrap() {
  const app = await NestFactory.create(QuestServerModule.forRoot())
  await CoreUtils.connectRedis(app)
  await app.listen(config.quest.httpPort)
}
bootstrap()
