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
        
        public static Vector2 ActualSize(RectTransform rect)
        {
            Canvas can = rect.GetComponentInParent<Canvas>();
            var v = new Vector3[4];
            rect.GetWorldCorners(v);
            //method one
            //return new Vector2(v[3].x - v[0].x, v[1].y - v[0].y);

            //method two
            return RectTransformUtility.PixelAdjustRect(rect, can).size;
        }
    }
}