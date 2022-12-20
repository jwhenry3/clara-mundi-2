using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        private void Awake()
        {
            Instance = this;
        }


    }
}