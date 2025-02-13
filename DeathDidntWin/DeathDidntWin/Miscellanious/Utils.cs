using UnityEngine;

namespace DeathDidntWin.Miscellanious;

public static class Utils
{
    public static Rect CenterWindow(Rect windowRect)
    {
        windowRect.x = (Screen.width - windowRect.width) / 2f;
        windowRect.y = (Screen.height - windowRect.height) / 2f;
        return windowRect;
    }
}