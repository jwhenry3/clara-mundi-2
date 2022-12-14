using System;
using TMPro;
using UnityEngine.EventSystems;

namespace ClaraMundi
{
    public class TextUtils
    {

        public static string ConvertToWords(string value)
        {
            string newValue = "";
            int count = 0;
            foreach (Char c in value)
            {
                if (count > 0 && Char.IsUpper(c))
                    newValue += " ";
                newValue += c;
                count++;
            }
            return newValue;
        }

        public static string GetLinkUnder(TextMeshProUGUI Text, PointerEventData eventData)
        {
            int linkIndex =
                TMP_TextUtilities.FindIntersectingLink(Text, eventData.position, eventData.pressEventCamera);
            if (linkIndex == -1) return "";
            var linkInfo = Text.textInfo.linkInfo[linkIndex];
            return linkInfo.GetLinkID();
        }
    }
}