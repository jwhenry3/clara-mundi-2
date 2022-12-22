using System;
using System.Collections.Generic;
using System.Linq;
using Unisave.Facades;
using Unisave.Facets;
using UnityEngine;

namespace Backend.App
{
    public class GameServerFacet : Facet
    {
        private readonly string[] RaceOptions = new[] { "human" };
        private readonly string[] GenderOptions = new[] { "male", "female" };
        public List<GameServerEntity> GetServers()
        {
            return DB.TakeAll<GameServerEntity>().Get();
        }

        public List<GameServerEntity> GetServersForRegion(string region)
        {
            return DB.TakeAll<GameServerEntity>().Filter((s) => s.Region == region).Get();
        }

        public GameServerEntity GetOnlineServerForRegion(string region)
        {
            return DB.TakeAll<GameServerEntity>().Filter((s) => s.Region == region && s.Status).First();
        }

        public bool SaveServer(string masterServerToken, GameServerModel server)
        {
            if (string.IsNullOrEmpty(masterServerToken)) return false;
            if (!ValuesAreProvided(new[] { server.Name, server.Region, server.Host }))
                return false;
            var existing = DB.TakeAll<GameServerEntity>().Filter((s) => s.Name == server.Name).First();
            
            var entity = existing ?? new GameServerEntity();
            entity.Name = server.Name;
            entity.Region = server.Region;
            entity.Status = (bool)server.Status;
            entity.Host = server.Host;
            entity.Port = server.Port;
            entity.PlayerCount = server.PlayerCount;
            entity.PlayerCapacity = server.PlayerCapacity;
            entity.Save();
            return true;
        }

        public void ResetServers()
        {
            var servers = DB.TakeAll<GameServerEntity>().Filter((s) => s.PlayerCount > 0).Get();
            foreach (var server in servers)
            {
                server.PlayerCount = 0;
                server.Save();
            }
        }
        public static bool ValuesAreProvided(string[] values) => values.All(value => !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value));

    }
}