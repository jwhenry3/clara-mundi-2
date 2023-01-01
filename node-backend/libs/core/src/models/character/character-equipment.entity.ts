import { Column, Entity, ManyToOne, PrimaryGeneratedColumn } from 'typeorm'

import { CharacterClassEntity } from './character-class.entity'

@Entity('character_class_equipment')
export class CharacterEquipmentEntity {
  @PrimaryGeneratedColumn('uuid')
  entityId: string
  @ManyToOne(() => CharacterClassEntity, (c) => c.equipment)
  characterClass: CharacterClassEntity
  @Column('varchar')
  slot: string
  @Column('varchar')
  itemId: string
}
