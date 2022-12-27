import { NestFactory } from '@nestjs/core'

import { LoginServerModule } from './login-server.module'

async function bootstrap() {
  const app = await NestFactory.create(LoginServerModule.forRoot())
  await app.listen(3000)
}
bootstrap()
