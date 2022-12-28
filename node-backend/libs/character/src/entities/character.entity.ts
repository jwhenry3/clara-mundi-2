import { Column, Entity, PrimaryColumn, PrimaryGeneratedColumn, Unique } from 'typeorm'

@Unique('name', ['name'])
@Entity('character')
export class CharacterEntity {
  @Column('varchar')
  accountId: string
  @PrimaryColumn('varchar')
  name: string
  @Column('varchar')
  gender: string
  @Column('varchar')
  race: string
  @Column('varchar')
  area: string
  @Column('decimal')
  position_x: number
  @Column('decimal')
  position_y: number
  @Column('decimal')
  position_z: number
  @Column('decimal')
  rotation: number

  @Column('int')
  level: number
  @Column('longint')
  exp: number

  @Column('int')
  lastConnected: number
  @Column('int')
  lastDisconnected: number

  @Column('tinyint')
  hasConnectedBefore: boolean
}
