import { AuthModule } from '@app/auth'
import { Module } from '@nestjs/common'

import { CharacterService } from './character.service'
import { CharacterController } from './character/character.controller'

@Module({
  imports: [AuthModule],
  providers: [CharacterService],
  exports: [CharacterService],
  controllers: [CharacterController],
})
export class CharacterModule {}
