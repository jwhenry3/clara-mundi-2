import { CharacterClassEntity, CharacterEntity, CharacterEquipmentEntity, CharacterItemInstanceEntity } from '@app/core'
import { Module } from '@nestjs/common'
import { TypeOrmModule } from '@nestjs/typeorm'

import { CharacterService } from './character.service'

@Module({
  imports: [
    TypeOrmModule.forFeature([
      CharacterEntity,
      CharacterClassEntity,
      CharacterEquipmentEntity,
      CharacterItemInstanceEntity,
    ]),
  ],
  providers: [CharacterService],
  exports: [CharacterService, TypeOrmModule],
})
export class CharacterModule {}
