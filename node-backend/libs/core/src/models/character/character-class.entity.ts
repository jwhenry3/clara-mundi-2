import { Column, Entity, JoinColumn, ManyToOne, OneToMany, OneToOne, PrimaryGeneratedColumn, Relation } from 'typeorm'

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

  @Column('int')
  level: number = 1
  @Column('bigint')
  exp: number = 0

  @Column('tinyint')
  isCurrent: boolean = false

  @OneToMany(() => CharacterEquipmentEntity, (e) => e.characterClass, {
    cascade: true,
    eager: true,
  })
  equipment: Relation<CharacterEquipmentEntity>[]
}
