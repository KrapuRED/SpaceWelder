using UnityEngine;
using System.Collections.Generic;

public static class RailUtils
{
    public static Direction Opposite(Direction d) => d switch
    {
        Direction.Right => Direction.Left,
        Direction.Left => Direction.Right,
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        _ => d
    };

    public static bool IsCorner(List<Direction> dirs)
    {
        if(dirs.Count != 2) return false;
        return dirs[0] != Opposite(dirs[1]);
    }
}
