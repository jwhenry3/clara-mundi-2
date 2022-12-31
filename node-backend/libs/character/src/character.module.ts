import { Module } from '@nestjs/common'
import { TypeOrmModule } from '@nestjs/typeorm'

import { CharacterService } from './character.service'
import { CharacterClassEntity } from './entities/character-class.entity'
import { CharacterEquipmentEntity } from './entities/character-equipment.entity'
import { CharacterEntity } from './entities/character.entity'

@Module({
  imports: [
    TypeOrmModule.forFeature([
      CharacterEntity,
      CharacterClassEntity,
      CharacterEquipmentEntity,
    ]),
  ],
  providers: [CharacterService],
  exports: [CharacterService, TypeOrmModule],
})
export class CharacterModule {}
