using System.Collections.Generic;
using UnityEngine;

namespace ClaraMundi
{
    public class EntityManager : MonoBehaviour
    {
        public Dictionary<string, Entity> Entities = new();
        public static EntityManager Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}