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
            if (verticalPosition < Screen.height / 2)
                return -1;
            return 1;
        }
    }
}