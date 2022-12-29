import { Controller, Delete, Get, Param, Post, Req, Res } from '@nestjs/common'
import { Request, Response } from 'express'

import { AuthService } from '../../../libs/auth/src/auth.service'
import { CharacterService } from '../../../libs/character/src/character.service'

const unauthorized = {
  status: false,
  reason: 'not-authorized',
}
@Controller('login-server')
export class LoginServerController {
  constructor(private auth: AuthService, private character: CharacterService) {}

  @Post('login')
  async login(@Req() req: Request, @Res() res: Response) {
    const { email, password } = req.body ?? { email: '', password: '' }

    const result = await this.auth.loginClient(email, password)
    this.respond(res, this.getStatus(result.reason), result)
  }

  @Post('register')
  async register(@Req() req: Request, @Res() res: Response) {
    const { email, password } = req.body ?? { email: '', password: '' }

    const result = await this.auth.registerClient(email, password)
    this.respond(res, this.getStatus(result.reason), result)
  }

  @Get('characters/list')
  async getCharacters(@Req() req: Request, @Res() res: Response) {
    const { token } = req.query ?? { token: '' }
    const accountId = await this.auth.getAccountId(token as string)
    if (!accountId)
      return this.respond(res, 401, { ...unauthorized, characters: [] })

    const result = await this.character.getCharacters(accountId)
    this.respond(res, this.getStatus(result.reason), result)
  }

  @Post('characters/create')
  async createCharacter(@Req() req: Request, @Res() res: Response) {
    const { token, name, race, gender } = (req.body ?? {
      token: '',
      race: 'human',
      name: '',
      gender: 'male',
    }) as Record<string, string>

    const accountId = await this.auth.getAccountId(token as string)
    if (!accountId)
      return this.respond(res, 401, { ...unauthorized, character: null })

    const options = { name, race, gender }
    const result = await this.character.createCharacter(accountId, options)
    this.respond(res, this.getStatus(result.reason), result)
  }

  @Delete('characters/:name')
  async deleteCharacter(
    @Req() req: Request,
    @Res() res: Response,
    @Param('name') name: string,
  ) {
    const { token } = req.query ?? { token: '' }

    const accountId = await this.auth.getAccountId(token as string)
    if (!accountId) return this.respond(res, 401, unauthorized)

    const result = await this.character.deleteCharacter(
      accountId,
      (name ?? '') as string,
    )
    this.respond(res, this.getStatus(result.reason), result)
  }

  respond(res: Response, status: number, body: any) {
    res.status(status)
    res.send(body)
  }

  getStatus(reason: string) {
    switch (reason) {
      case 'not-found':
        return 404
      case 'conflict':
        return 409
      case '':
        return 200
      default:
        return 406
    }
  }
}
