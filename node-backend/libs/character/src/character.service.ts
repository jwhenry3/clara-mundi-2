import { CharacterClassEntity, CharacterEntity } from '@app/core'
import { Injectable } from '@nestjs/common'
import { InjectRepository } from '@nestjs/typeorm'
import { Raw, Repository } from 'typeorm'

export class CreateCharacterOptions {
  name: string
  gender: string = 'male'
  race: string = 'human'
  startingClass: string = 'adventurer'
}
@Injectable()
export class CharacterService {
  constructor(
    @InjectRepository(CharacterEntity)
    private repo: Repository<CharacterEntity>,
    @InjectRepository(CharacterClassEntity)
    private classRepo: Repository<CharacterClassEntity>,
  ) {}

  async saveCharacter(
    character: CharacterEntity,
    allRelations: boolean = false,
  ) {
    if (!allRelations) return await this.repo.save(character)
    await this.repo.save(character)
  }
  async createCharacter(accountId: string, options: CreateCharacterOptions) {
    if (!accountId) {
      return {
        status: false,
        reason: 'invalid-accountId',
        character: null,
      }
    }
    if (!options.name) {
      return {
        status: false,
        reason: 'invalid-name',
        character: null,
      }
    }
    const existing = await this.findByName(options.name)
    if (!!existing) {
      return {
        status: false,
        reason: 'conflict',
        character: null,
      }
    }
    if (!['adventurer'].includes(options.startingClass.toLowerCase())) {
      return {
        status: false,
        reason: 'invalid-class',
        character: null,
      }
    }
    let character = this.repo.create({
      accountId,
      name: options.name,
      race: ['human'].includes(options.race) ? options.race : 'human',
      gender: ['male', 'female'].includes(options.gender)
        ? options.gender
        : 'male',
      characterClasses: [],
    })

    const characterClass = this.classRepo.create({
      classId: options.startingClass.toLowerCase(),
      character,
      level: 1,
      isCurrent: 1,
    })
    character.characterClasses.push(characterClass)
    character = await this.repo.save(character)
    return {
      status: true,
      reason: '',
      character,
    }
  }

  async getCharacters(accountId: string) {
    if (!accountId) {
      return {
        status: false,
        reason: 'invalid-accountId',
        characters: [],
      }
    }
    const characters = await this.getCharactersForList(accountId)
    console.log(characters)
    return {
      status: true,
      reason: '',
      characters,
    }
  }

  async deleteCharacter(accountId: string, name: string) {
    const character = await this.findByAccountAndName(accountId, name)
    if (!character) {
      return {
        status: false,
        reason: 'not-found',
      }
    }
    await this.repo.delete(character)

    return {
      status: true,
      reason: '',
    }
  }
  async getCharacterByAccountAndName(
    accountId: string,
    name: string,
    includeClasses: boolean = false,
  ) {
    const character = await this.findByAccountAndName(accountId, name)
    if (!character) {
      return {
        status: false,
        reason: 'not-found',
        character: null,
      }
    }
    return {
      status: true,
      reason: '',
      character,
    }
  }

  async getCharacter(name: string) {
    const character = await this.findByName(name)
    if (!character) {
      return {
        status: false,
        reason: 'not-found',
        character: null,
      }
    }
    return {
      status: true,
      reason: '',
      character,
    }
  }

  async updateCharacter(
    name: string,
    area: string,
    position_x: number,
    position_y: number,
    position_z: number,
    rotation: number,
  ) {
    return await this.repo
      .createQueryBuilder('character')
      .update({
        area,
        position_x,
        position_y,
        position_z,
        rotation,
      })
      .where({ name })
      .execute()
  }
  async getCharactersForList(accountId: string) {
    return await this.repo
      .createQueryBuilder('character')
      .select([
        'character.name as name',
        'character.gender as gender',
        'character.race as race',
        'character.area as area',
        'class.level as level',
        'class.exp as exp',
        'class.classId as classId',
      ])
      .where('character.accountId = :accountId', { accountId })
      .leftJoinAndSelect(
        'character.characterClasses',
        'class',
        'class.isCurrent = 1',
      )
      .execute()
  }

  async searchCharacters(term: string) {
    return await this.repo
      .createQueryBuilder('character')
      .select([
        'character.name as name',
        'character.gender as gender',
        'character.race as race',
        'character.area as area',
        'class.level as level',
        'class.exp as exp',
        'class.classId as classId',
      ])
      .where('character.name like :term', { term: `%${term}%` })
      .leftJoinAndSelect(
        'character.characterClasses',
        'class',
        'class.isCurrent = 1',
      )
      .getMany()
  }
  private findByAccountAndName(
    accountId: string,
    name: string,
    includeClasses = false,
  ) {
    if (includeClasses) {
      return this.repo.findOne({
        where: {
          accountId,
          name: Raw((alias) => `LOWER(${alias}) = LOWER(:value)`, {
            value: name,
          }),
        },
        relations: ['characterClasses', 'characterClasses.equipment'],
      })
    }
    return this.repo.findOneBy({
      accountId,
      name: Raw((alias) => `LOWER(${alias}) = LOWER(:value)`, {
        value: name,
      }),
    })
  }
  private findByName(name: string) {
    return this.repo.findOneBy({ name })
  }
}
