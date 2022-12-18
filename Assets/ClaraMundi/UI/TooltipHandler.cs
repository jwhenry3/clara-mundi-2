using System;
using UnityEngine;

namespace ClaraMundi
{
    
    public class TooltipHandler : MonoBehaviour
    {
        public static TooltipHandler Instance;
        public ItemTooltipUI ItemTooltipUI;

        private void Awake()
        {
            Instance = this;
        }
    }
}