import { AuthService } from '@app/auth'
import { WebSocketClient, WebsocketUtils } from '@app/websocket'
import { ConnectedSocket, WebSocketServer } from '@nestjs/websockets'
import {
  MessageBody,
  OnGatewayConnection,
  OnGatewayDisconnect,
  SubscribeMessage,
  WebSocketGateway,
} from '@nestjs/websockets'
import { create } from 'guid'
import { Server, WebSocket } from 'ws'

const url = require('url')
export interface ServerEntry {
  label: string
  region: string
  host: string
  port: number
  status: boolean
  playerCapacity: number
  currentPlayers: number
}
@WebSocketGateway(3001)
export class MasterServerGateway
  implements OnGatewayConnection, OnGatewayDisconnect
{
  static serverList: Record<string, ServerEntry> = {}

  @WebSocketServer()
  server: Server

  authorized: WebSocketClient[] = []

  serversByClient: Record<string, ServerEntry> = {}

  constructor(private auth: AuthService) {}

  handleDisconnect(client: WebSocketClient) {
    console.log('client disconnected, clean up')
    const index = this.authorized.indexOf(client)
    if (index > -1) this.authorized.splice(index, 1)
    if (client.id in this.serversByClient) {
      const server = this.serversByClient[client.id]
      server.status = false
      this.broadcastServerList()
      delete this.serversByClient[client.id]
    }
  }

  handleConnection(client: any, message: any) {
    const parameters = url.parse(message.url, true)

    if (!this.auth.validateServer(parameters.query.token as string)) {
      client.close(1008)
      return
    }
    client.send(JSON.stringify({ event: 'authorized', data: '' }))
  }

  @SubscribeMessage('update')
  handleAuth(@ConnectedSocket() client: any, @MessageBody() body: string) {
    const hostParts = client._socket.remoteAddress.split(':')
    const host = hostParts.pop()
    client.id = create().toString()
    this.authorized.push(client)
    const data = JSON.parse(body) as ServerEntry
    const entry: ServerEntry = {
      label: data.label,
      region: data.region,
      host: host,
      port: data.port,
      status: true,
      playerCapacity: data.playerCapacity ?? 0,
      currentPlayers: data.currentPlayers ?? 0,
    }
    this.serversByClient[client.id] = entry
    MasterServerGateway.serverList[data.label] = entry
    this.broadcastServerList()
  }

  broadcastServerList() {
    WebsocketUtils.broadcast(
      this.authorized,
      JSON.stringify({
        event: 'server-list',
        data: JSON.stringify(Object.values(MasterServerGateway.serverList)),
      }),
    )
  }
}
