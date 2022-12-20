using System.Collections.Generic;
using ClaraMundi;
using Unisave.Entities;
using Unisave.Facades;
using Unisave.Facets;

namespace Backend.App
{
    public class InventoryFacet : Facet
    {
        public List<ItemEntity> GetItems(string characterId)
        {
            AccountEntity account = Auth.GetPlayer<AccountEntity>();
            if (account == null) return new List<ItemEntity>();
            var character = DB.Find<CharacterEntity>(characterId);
            if (character.Account.TargetId != account.EntityId) return new();
            return DB.TakeAll<ItemEntity>().Filter((entity => entity.Character.TargetId == characterId)).Get();
        }

        public List<ItemEntity> ServerGetItems(string serverToken, string characterId)
        {
            return DB.TakeAll<ItemEntity>().Filter((entity => entity.Character.TargetId == characterId)).Get();
        } 

        public string AddItem(string serverToken,  string characterId, ItemInstanceModel model)
        {
            var character = DB.Find<CharacterEntity>(characterId);
            if (character == null) return "";
            var item = new ItemEntity
            {
                Character = character,
                ItemId = model.ItemId,
                Quantity = model.Quantity,
                StorageId = model.StorageId,
                IsEquipped = model.IsEquipped,
            };
            item.Save();
            return item.EntityId;
        }

        public bool UpdateItem(string serverToken, string characterId, ItemInstanceModel model)
        {
            var item = DB.Find<ItemEntity>(model.ItemInstanceId);
            if (item == null) return false;
            if (item.Character.TargetId != characterId) return false;
            item.Quantity = model.Quantity;
            item.IsEquipped = model.IsEquipped;
            item.StorageId = model.StorageId;
            item.Save();
            return true;
        }

        public bool RemoveItem(string serverToken,  string characterId, string instanceId)
        {
            var item = DB.Find<ItemEntity>(instanceId);
            if (item == null) return false;
            if (item.Character.TargetId != characterId) return false;
            item.Delete();
            return true;
        }

        public Dictionary<string,string> SetItemsForStorage(string serverToken,  string characterId, string storageId, List<ItemInstanceModel> models)
        {
            var character = DB.Find<CharacterEntity>(characterId);
            // empty mapping indicates nothing was added
            if (character == null) return new();
            var items = DB.TakeAll<ItemEntity>().Filter((entity => entity.Character.TargetId == characterId && storageId == entity.StorageId)).Get();
            foreach (var item in items)
            {
                var model = models.Find((m) => item.EntityId == m.ItemInstanceId);
                if (model  == null)
                    item.Delete();
                else
                {
                    models.Remove(model);
                    item.Quantity = model.Quantity;
                    item.IsEquipped = model.IsEquipped;
                    item.Save();
                }
            }

            var idMapping = new Dictionary<string, string>();
            foreach (var model in models)
            {
                var newItem = new ItemEntity
                {
                    StorageId =  storageId,
                    ItemId = model.ItemId,
                    Quantity = model.Quantity,
                    IsEquipped = model.IsEquipped,
                    Character =  character
                };
                newItem.Save();
                idMapping.Add(model.ItemInstanceId, newItem.EntityId);
            }
            // return mapping of itemInstanceId to EntityId so we can keep them in sync
            return idMapping;
        }
    }
}