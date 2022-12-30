import { CoreUtils } from '@app/core'
import { config } from '@app/core'
import { NestFactory } from '@nestjs/core'

import { LoginServerModule } from './login-server.module'

async function bootstrap() {
  const app = await NestFactory.create(LoginServerModule.forRoot())
  await CoreUtils.connectRedis(app)
  await app.listen(config.login.httpPort)
}
bootstrap()
