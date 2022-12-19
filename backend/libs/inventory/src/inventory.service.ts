import { Injectable } from '@nestjs/common'

export interface ItemInstance {
  itemInstanceId: string
  itemId: string
  storageId: string
  quantity: number
  isEquipped: boolean
}
@Injectable()
export class InventoryService {
  public getItems(characterName: string, storageId: string) {}
  public getQuantityOfInstances(characterName: string, instanceIds: string[]) {}
  public getQuantityOfItems(characterName: string, itemIds: string[]) {}
  public getEquippedItems(characterName: string) {}

  public addItems(characterName: string, items: ItemInstance[]) {}
  public updateItems(characterName: string, items: ItemInstance[]) {}
  public removeItems(characterName: string, itemInstanceIds: string[]) {}
}
