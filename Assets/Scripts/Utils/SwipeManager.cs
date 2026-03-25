using UnityEngine;

public static class SwipeManager
{
    public static SwipeDirection GetSwipeDirection(Vector2 fingerDown,Vector2 fingerUp,float SWIPE_THRESHOLD = 0.5f)
    {
        if (Mathf.Abs(fingerDown.y - fingerUp.y) > SWIPE_THRESHOLD && Mathf.Abs(fingerDown.y - fingerUp.y) > Mathf.Abs(fingerDown.x - fingerUp.x))
        {
            if (fingerDown.y - fingerUp.y > 0)
            {
                return SwipeDirection.Bottom;
            }
            else if (fingerDown.y - fingerUp.y < 0)
            {
                return SwipeDirection.Top;
            }
        }
        else if (Mathf.Abs(fingerDown.x - fingerUp.x) > SWIPE_THRESHOLD && Mathf.Abs(fingerDown.x - fingerUp.x) > Mathf.Abs(fingerDown.y - fingerUp.y))
        {
            if (fingerDown.x - fingerUp.x > 0)
            {
                return SwipeDirection.Left;
            }
            else if (fingerDown.x - fingerUp.x < 0)
            {
                return SwipeDirection.Right;
            }
        }
        return SwipeDirection.None;
    }
}
[System.Serializable]
public enum SwipeDirection
{
    None,
    Left,
    Top,
    Right,
    Bottom
}