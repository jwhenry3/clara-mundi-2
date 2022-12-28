import { Controller, Get, Req, Res } from '@nestjs/common'
import { Post } from '@nestjs/common'
import { Request, Response } from 'express'

import { AuthService } from '../../../libs/auth/src/auth.service'
import { LoginServerService } from './login-server.service'

@Controller('login-server')
export class LoginServerController {
  constructor(private auth: AuthService) {}

  @Post('login')
  async login(@Req() req: Request, @Res() res: Response) {
    console.log(req.body)
    const { email, password } = req.body ?? { email: '', password: '' }
    const result = await this.auth.loginClient(email, password)
    res.status(this.getStatus(result.reason))
    res.send(result)
  }

  @Post('register')
  async register(@Req() req: Request, @Res() res: Response) {
    console.log(req.body)
    const { email, password } = req.body ?? { email: '', password: '' }

    const result = await this.auth.registerClient(email, password)
    console.log(result)
    res.status(this.getStatus(result.reason))
    res.send(result)
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
