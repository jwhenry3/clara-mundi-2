import { INestApplication } from '@nestjs/common'
import { ClientsModule, Transport } from '@nestjs/microservices'

export class CoreUtils {
  static async connectRedis(app: INestApplication) {
    const microservice = app.connectMicroservice({
      transport: Transport.REDIS,
      options: {
        host: process.env.REDIS_HOST,
        port: parseInt(process.env.REDIS_PORT),
        username: process.env.REDIS_USER,
        password: process.env.REDIS_PASS,
      },
    })
    await app.startAllMicroservices()
  }
  static importClient(serviceName: string) {
    return ClientsModule.register([
      {
        name: serviceName,
        transport: Transport.REDIS,
        options: {
          host: process.env.REDIS_HOST,
          port: parseInt(process.env.REDIS_PORT),
        },
      },
    ])
  }
}
