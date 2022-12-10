using UnityEngine;

namespace ClaraMundi
{
    public class ScreenUtils
    {


        public static int GetHorizontalWithMostSpace(float horizontalPosition)
        {
            if (horizontalPosition > Screen.width / 2)
                return -1;
            return 1;
        }
        public static int GetVerticalWithMostSpace(float verticalPosition)
        {
            if (Screen.height - verticalPosition < 64)
                return -1;
            if (verticalPosition < 64)
                return 1;
            return 0;
        }
    }
}