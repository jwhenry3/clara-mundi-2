import {
    MessageBody,
    OnGatewayConnection,
    OnGatewayDisconnect,
    SubscribeMessage,
    WebSocketGateway,
} from '@nestjs/websockets'
import { WebSocket } from 'ws'

@WebSocketGateway(3003)
export class QuestGateway implements OnGatewayConnection, OnGatewayDisconnect {
  handleDisconnect(client: WebSocket) {
    console.log('client disconnected, clean up')
  }
  handleConnection(client: any, ...args: any[]) {
    console.log('Received connection, validate client')
    client.send(
      JSON.stringify({ eventName: 'test', data: { property: 'value' } }),
    )
  }

  @SubscribeMessage('events')
  handleEvent(@MessageBody('id') id: number) {}
}
