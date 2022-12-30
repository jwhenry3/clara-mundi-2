import { CharacterModel, CharacterService } from '@app/character'
import { Controller, Get } from '@nestjs/common'
import { Post } from '@nestjs/common'
import { Req } from '@nestjs/common'
import { Res } from '@nestjs/common'
import { Param } from '@nestjs/common'
import { Inject } from '@nestjs/common'
import { DEFAULT_FACTORY_CLASS_METHOD_KEY } from '@nestjs/common/module-utils/constants'
import { ClientProxy, MessagePattern } from '@nestjs/microservices'
import { Request, Response } from 'express'
import { from } from 'rxjs'

import { CharacterServerService } from './character-server.service'

@Controller('character-server')
export class CharacterServerController {
  characters: Record<string, CharacterModel> = {}

  constructor(
    @Inject('CHARACTER_SERVICE') private client: ClientProxy,
    private character: CharacterService,
  ) {}

  @Get('characters/search')
  searchCharacters(@Req() request: Request, @Res() response: Response) {
    const term = (request.query.term ?? '') as string
    if (term.length < 3) {
      response.status(406)
      response.send({
        status: false,
        reason: 'term-length',
        characters: [],
      })
      return
    }
    const found = Object.values(this.characters).filter((character) =>
      character.name.toLowerCase().includes(term.toLowerCase()),
    )
    response.status(200)
    response.send({
      status: true,
      reason: '',
      characters: found,
    })
  }

  @MessagePattern('character:get')
  getCharacter(name: string) {
    if (!(name in this.characters)) return null
    return this.characters[name]
  }
}
