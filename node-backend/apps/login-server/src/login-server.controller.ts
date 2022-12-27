import { Controller, Get, Req, Res } from '@nestjs/common'
import { Post } from '@nestjs/common'
import { Request, Response } from 'express'

import { LoginServerService } from './login-server.service'

@Controller('login-server')
export class LoginServerController {
  constructor(private readonly loginServerService: LoginServerService) {}

  @Get()
  getHello(): string {
    return this.loginServerService.getHello()
  }

  @Post('login')
  login(@Req() req: Request, @Res() res: Response) {
    console.log(req.body)
    res.status(200)
    res.send({
      status: true,
      reason: 'Successfully Logged In!',
    })
  }
  @Post('register')
  register(@Req() req: Request, @Res() res: Response) {
    console.log(req.body)
    res.status(200)
    res.send({
      status: true,
      reason: 'Successfully Registered!',
    })
  }
}
