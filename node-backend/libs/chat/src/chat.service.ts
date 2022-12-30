import { Injectable } from '@nestjs/common'

import { CharacterModel } from '../../character/src/character.model'
import { CharacterEntity } from '../../character/src/entities/character.entity'
import { WebSocketClient } from '../../websocket/src/websocket.client'
import { WebsocketRoomManager } from '../../websocket/src/websocket.room-manager'
import { WebSocketMessage } from '../../websocket/src/websocket.utils'
import { ChatMessage } from './chat.message'

@Injectable()
export class ChatService {
  charactersByClientId: Record<string, CharacterModel> = {}
  rooms = new WebsocketRoomManager()

  join(client: WebSocketClient, name: string) {
    return this.rooms.joinRoom(client, 'chat:' + name)
  }
  leave(client: WebSocketClient, name: string) {
    return this.rooms.leaveRoom(client, 'chat:' + name)
  }
  send(client: WebSocketClient, name: string, message: ChatMessage) {
    if (!(name in this.rooms)) return false
    if (!this.rooms[name].hasMember(client.id)) return false
    if (client.id in this.charactersByClientId) {
      message.senderName = this.charactersByClientId[client.id].name
      message.senderArea = this.charactersByClientId[client.id].area
    }
    return this.rooms[name].send(new WebSocketMessage('chat:message', message))
  }
}
