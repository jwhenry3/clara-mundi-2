import { INestApplication } from '@nestjs/common'
import { ClientsModule, Transport } from '@nestjs/microservices'

export class CoreUtils {
  static async connectRedis(app: INestApplication) {
    const microservice = app.connectMicroservice({
      transport: Transport.NATS,
      options: {
        servers: ['nats://localhost:4222'],
      }
    })
    await app.startAllMicroservices()
  }
  static importClient(serviceName: string) {
    return ClientsModule.register([
      {
        name: serviceName,
        transport: Transport.NATS,
        options: {
          servers: ['nats://localhost:4222'],
        },
      },
    ])
  }
}
