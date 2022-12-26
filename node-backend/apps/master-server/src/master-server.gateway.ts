import { AuthService } from '@app/auth'
import { ConnectedSocket } from '@nestjs/websockets'
import {
    MessageBody,
    OnGatewayConnection,
    OnGatewayDisconnect,
    SubscribeMessage,
    WebSocketGateway,
} from '@nestjs/websockets'
import { debug } from 'console'
import { WebSocket } from 'ws'

@WebSocketGateway(3001)
export class MasterServerGateway
  implements OnGatewayConnection, OnGatewayDisconnect
{
  constructor(private auth: AuthService) {}

  authorized = []

  static serverList = [
    {
      label: 'test',
      region: 'Vilterra',
      host: 'localhost',
      port: 7700,
      status: true,
      playerCapacity: 100,
      currentPlayers: 0,
    },
  ]
  handleDisconnect(client: WebSocket) {
    console.log('client disconnected, clean up')
  }
  handleConnection(client: WebSocket, ...args: any[]) {
    console.log('Received connection, validate client')
    client.send(JSON.stringify({ event: 'authorize', data: {} }))
  }
  @SubscribeMessage('authorize')
  handleAuth(
    @ConnectedSocket() client: WebSocket,
    @MessageBody() data: { token: string },
  ) {
    if (this.auth.validateServer(data.token)) {
      this.authorized.push(client)
      client.send(JSON.stringify({ event: 'authorized', data: {} }))
      client.send(
        JSON.stringify({
          event: 'server-list',
          data: {
            list: JSON.stringify(MasterServerGateway.serverList),
          },
        }),
      )
    } else client.close(1008) // normal close
  }

  @SubscribeMessage('events')
  handleEvent(
    @ConnectedSocket() client: WebSocket,
    @MessageBody() data: { token: string },
  ) {
    console.log('Received event', data)
  }
}
