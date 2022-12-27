import { Module } from '@nestjs/common'
import { TypeOrmModule } from '@nestjs/typeorm'

import { AuthService } from './auth.service'
import { AccountEntity } from './entities/account.entity'
import { CharacterEntity } from './entities/character.entity'

@Module({
  imports: [TypeOrmModule.forFeature([AccountEntity, CharacterEntity])],
  providers: [AuthService],
  exports: [AuthService],
})
export class AuthModule {}
