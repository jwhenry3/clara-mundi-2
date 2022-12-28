import { Module } from '@nestjs/common'
import { TypeOrmModule } from '@nestjs/typeorm'

import { CharacterService } from './character.service'
import { CharacterEntity } from './entities/character.entity'

@Module({
  imports: [TypeOrmModule.forFeature([CharacterEntity])],
  providers: [CharacterService],
  exports: [CharacterService, TypeOrmModule],
})
export class CharacterModule {}
