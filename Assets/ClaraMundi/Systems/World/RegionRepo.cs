using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
    [CreateAssetMenu(fileName = "RegionRepo", menuName = "Clara Mundi/World/RegionRepo")]
    [Serializable]
    public class RegionRepo : ScriptableObject
    {
        
        public Dictionary<string, Region> Regions;
        // global zone lookup by scene key
        public Dictionary<string, Zone> Zones;
        public Region[] RegionList;
        
        public void OnEnable()
        {
            Regions = new Dictionary<string, Region>();
            Zones = new Dictionary<string, Zone>();
            if (RegionList == null) return;
            foreach (var region in RegionList)
            {
                Regions.Add(region.Name, region);
                if (region.Zones == null) return;
                foreach (var zone in region.Zones)
                {
                    if (zone == null) return;
                    region.ZoneDict[zone.Key] = zone;
                    zone.Region = region;
                    Zones[zone.Key] = zone;
                }
            }
        }
    }
}