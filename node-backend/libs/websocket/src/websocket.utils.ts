import { Server } from 'ws'

import { WebSocketClient } from './websocket.client'

export class WebsocketUtils {
  static broadcast(clients: WebSocketClient[], event: string) {
    for (const client of clients) {
      client.send(event)
    }
  }
}
