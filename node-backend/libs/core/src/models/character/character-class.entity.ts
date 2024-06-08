import {
  Column,
  Entity,
  JoinColumn,
  ManyToOne,
  OneToMany,
  OneToOne,
  PrimaryGeneratedColumn,
  Relation,
} from 'typeorm'

import { CharacterEquipmentEntity } from './character-equipment.entity'
import { CharacterEntity } from './character.entity'

@Entity('character_class')
export class CharacterClassEntity {
  @PrimaryGeneratedColumn('uuid')
  characterClassId: string

  @ManyToOne(() => CharacterEntity, { eager: false })
  @JoinColumn({ name: 'characterId' })
  character: Relation<CharacterEntity>

  @Column('varchar', { default: 'adventurer' })
  classId: string = 'adventurer'

  @Column('int', { default: 1 })
  level: number = 1
  @Column('int', { default: 0 })
  exp: number = 0

  @Column('int', { default: 0 })
  isCurrent: number = 0

  @OneToOne(() => CharacterEquipmentEntity, (e) => e.characterClass, {
    cascade: true,
    eager: true,
  })
  equipment: Relation<CharacterEquipmentEntity>
}
