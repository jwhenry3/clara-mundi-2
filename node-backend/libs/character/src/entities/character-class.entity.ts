import { Column, Entity, JoinColumn, ManyToOne, OneToOne, PrimaryGeneratedColumn, Relation } from 'typeorm'

import { CharacterEquipmentEntity } from './character-equipment.entity'
import { CharacterEntity } from './character.entity'

@Entity('character_class')
export class CharacterClassEntity {
  @PrimaryGeneratedColumn('uuid')
  characterClassId: string

  @ManyToOne(() => CharacterEntity, (c) => c.characterClasses)
  @JoinColumn()
  character: Relation<CharacterEntity>

  @Column('varchar')
  classId: string
  @Column('tinyint')
  isCurrent: boolean = false

  @OneToOne(() => CharacterEquipmentEntity, (e) => e.characterClass)
  equipment: Relation<CharacterEquipmentEntity>
}
