﻿using System;

namespace ClaraMundi
{
    [Serializable]
    public class StatValue
    {
        public string ValueId = Guid.NewGuid().ToString();
        public StatType Type = StatType.Strength;
        // Flat amount to apply on top
        public float Amount = 0;
        // Percent increase/decrease
        public float Percent = 0;

        public float GetModifiedValue(float initial)
        {
            if (Amount != 0)
                return initial + Amount;
            if (Percent != 0)
                return initial + (initial * (Percent / 100));
            return initial;
        }
    }
}