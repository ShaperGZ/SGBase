using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMode
{
    public const int RULE = 111;
    public const int VISUAL = 222;
    public const int NAMES = 333;
    public const int NORMAL = 444;

}

public class DisplayManager
{
    public int currMode = DisplayMode.NORMAL;
    public int lastMode = DisplayMode.NORMAL;

    public void SetMode(int mode)
    {
        currMode = mode;
    }

}