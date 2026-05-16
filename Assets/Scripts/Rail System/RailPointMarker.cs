using UnityEngine;

public class RailPointMarker : MonoBehaviour
{
    public Rail rail;
    public int pointIndex;

    public RailPoint GetRailPoint()
    {
        if (rail == null || rail.points == null) return null;
        if (pointIndex >= rail.points.Count) return null;
        return rail.points[pointIndex];
    }
}
