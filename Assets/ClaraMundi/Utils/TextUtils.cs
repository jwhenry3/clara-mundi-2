using System;

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
    }
}