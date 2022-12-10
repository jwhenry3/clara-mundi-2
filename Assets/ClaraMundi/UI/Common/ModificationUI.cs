using System;
using UnityEngine;
using TMPro;

namespace ClaraMundi
{
    public class ModificationUI : MonoBehaviour
    {
        public TextMeshProUGUI Label;

        public void SetValue(StatValue value)
        {
            Label.text = TextUtils.ConvertToWords(Enum.GetName(typeof(StatType), value.Type)) + "  " + GetValueString(value.Amount, value.Percent);
        }
        public void SetValue(AttributeValue value)
        {
            Label.text = TextUtils.ConvertToWords(Enum.GetName(typeof(AttributeType), value.Type)) + "  " + GetValueString(value.Amount, value.Percent);
        }

        string GetValueString(float amount, float percent)
        {
            if (amount != 0)
            {
                return amount > 0 ? "+" + amount : "-" + amount;
            }
            return percent >= 0 ? "+" + percent + "%" : "-" + percent + "%";
        }
    }
}