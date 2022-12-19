import { Injectable } from '@nestjs/common'

export interface CreationOptions {
  name: string
  race: string
  gender: string
}
@Injectable()
export class CharacterService {
  public getCharactersForAccount(accountName: string) {}
  public getCharacter(accountName: string, characterName: string) {}
  public addCharacter(accountName: string, createOptions: CreationOptions) {}
  public removeCharacter(accountName: string, characterName: string) {}
}
