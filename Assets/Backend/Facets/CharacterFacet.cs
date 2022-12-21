using System;
using System.Collections.Generic;
using System.Linq;
using Unisave.Facades;
using Unisave.Facets;
using UnityEngine;

namespace Backend.App
{
    public class CharacterFacet : Facet
    {
        private readonly string[] RaceOptions = new[] { "human" };
        private readonly string[] GenderOptions = new[] { "male", "female" };
        public List<CharacterEntity> GetCharacters()
        {
            AccountEntity account = Auth.GetPlayer<AccountEntity>();
            return account == null ? new List<CharacterEntity>() : GetByAccount(account);
        }

        public CharacterEntity ServerCharacterJoiningGameServer(string serverToken, string accountId, string token, string characterId)
        {
            var account = DB.Find<AccountEntity>(accountId);
            if (account == null) return null;
            if (account.token != token) return null;
            var character = GetById(characterId);
            if (character == null) return null;
            // the character is connecting to the server
            character.LastConnected = DateTime.UtcNow;
            character.HasConnectedBefore = true;
            character.Save();
            // empty mapping indicates nothing was added
            return character;
        }

        public void ServerCharacterLeavingGameServer(string serverToken,  string characterId)
        {
            var character = DB.Find<CharacterEntity>(characterId);
            if (character == null) return;
            character.LastDisconnected = DateTime.UtcNow;
            character.Save();
        }

        public bool CreateCharacter(CreateCharacterRequest request)
        {
            AccountEntity account = Auth.GetPlayer<AccountEntity>();
            if (account == null) return false;
            Debug.Log(request.Name + ", " + request.Gender + ", " + request.Race);
            if (!ValuesAreProvided(new string[] { request.Name, request.Gender, request.Race })) return false;
            if (!ValueMatches(RaceOptions, request.Race.ToLower())) return false;
            if (!ValueMatches(GenderOptions, request.Gender.ToLower())) return false;
            request.Name = request.Name.ToLower();
            request.Gender = request.Gender.ToLower();
            request.Race = request.Race.ToLower();
            // TBD: add character customization options to the request
            var existing = GetByName(request.Name);
            if (existing != null) return false;
            var character = new CharacterEntity()
            {
                Account = account,
                Name = request.Name,
                Gender = request.Gender,
                Race = request.Race,
                Area = "Rein",
                Position = Vector3.zero,
                Level = 1,
                TotalExp =  0
            };
            character.Save();
            return true;
        }

        public bool DeleteCharacter(string characterName)
        {
            if (!ValuesAreProvided(new string[] { characterName })) return false;
            characterName = characterName.ToLower();
            AccountEntity account = Auth.GetPlayer<AccountEntity>();
            if (account == null) return false;
            var existing = GetByNameAndAccount(characterName, account);
            if (existing == null) return false;
            // cannot delete the character when he has not disconnected from the game
            if (existing.HasConnectedBefore && existing.LastDisconnected < existing.LastConnected) return false;
            existing.Delete();
            // remove all attached data
            DB.TakeAll<ItemEntity>().Filter((entity => entity.Character == existing)).Get().ForEach((i) => i.Delete());
            DB.TakeAll<QuestCompletionEntity>().Filter((entity => entity.Character == existing)).Get().ForEach((i) => i.Delete());
            DB.TakeAll<QuestTaskProgressEntity>().Filter((entity => entity.Character == existing)).Get().ForEach((i) => i.Delete());
            return true;
        }

        public static bool ValuesAreProvided(string[] values) => values.All(value => !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value));
        public static bool ValueMatches(string[] options, string value) => options.Any(option => value == option);
        public static CharacterEntity GetByName(string Name) => DB.TakeAll<CharacterEntity>().Filter((entity) => entity.Name== Name).First();
        public static CharacterEntity GetByNameAndAccount(string Name, AccountEntity account) => DB.TakeAll<CharacterEntity>().Filter((entity) => entity.Name== Name && entity.Account == account).First();
        public static CharacterEntity GetById(string Id) => DB.Find<CharacterEntity>(Id);
        public static List<CharacterEntity> GetByAccount(AccountEntity account) => DB.TakeAll<CharacterEntity>().Filter((entity) => entity.Account == account).Get();
    }
}