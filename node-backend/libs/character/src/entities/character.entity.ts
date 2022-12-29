import { Column, Entity, PrimaryColumn, PrimaryGeneratedColumn, Unique } from 'typeorm'

@Unique('name', ['name'])
@Entity('character')
export class CharacterEntity {
  @Column('varchar')
  accountId: string
  @PrimaryColumn('varchar')
  name: string
  @Column('varchar')
  gender: string = 'male'
  @Column('varchar')
  race: string = 'human'
  @Column('varchar')
  area: string = 'Rein'
  @Column('decimal')
  position_x: number = 0
  @Column('decimal')
  position_y: number = 0
  @Column('decimal')
  position_z: number = 0
  @Column('decimal')
  rotation: number = 0

  @Column('int')
  level: number = 1
  @Column('longint')
  exp: number = 0

  @Column('int', { nullable: true })
  lastConnected: number
  @Column('int', { nullable: true })
  lastDisconnected: number

  @Column('tinyint')
  hasConnectedBefore: boolean = false
}
