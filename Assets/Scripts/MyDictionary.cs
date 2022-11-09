using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDictionary {

    public const char SHAPE_CUBE = 'C';
    public const char SHAPE_SPHERE = 'O';
    public const char SHAPE_CROSS = 'x';
    public const char SHAPE_CYLINDER_CROSS = 'X';

    public enum Platform
    {
        Nothing = 'N',
        High = 'H',
        Low = 'G',
        Stairs = 'S',
        Ramp = 'R',
        Tunnel = 'T',
        Start = 's',
        End = 'e'
    }

    public const int ENTRANCE_LEFT = 0;
    public const int ENTRANCE_UP = 1;
    public const int ENTRANCE_RIGHT = 2;
    public const int ENTRANCE_DOWN = 3;
}
