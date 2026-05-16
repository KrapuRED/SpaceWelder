using System.Collections.Generic;
using UnityEngine;

public enum PointType { Normal, Junction, Corner }
public enum Direction { Right, Left, Up, Down }

[System.Serializable]
public class RailPoint
{
    public Vector2 position;
    public PointType type;

    // Each point stores its neighbors per direction
    public Dictionary<Direction, RailPoint> connections = new();

    // For junction/corner: which directions are available
    public List<Direction> availableDirections = new();
}

[System.Serializable]
public class RailConnector
{
    public Rail fromRail;
    public Rail toRail;
    public Direction directionFromTo;
}

public class RailManager : MonoBehaviour
{
    public List<Rail> rails;
    public List<RailConnector> connectors;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        foreach (var rail in rails)
            rail.GeneratePoint();

        DetectIntersections();
        ApplyConnector();
        LogJunctions();
    }

    void LogJunctions()
    {
        foreach (var rail in rails)
            foreach (var point in rail.points)
                if (point.type != PointType.Normal)
                    Debug.Log($"[Junction] pos: {point.position} | type: {point.type} | dirs: {string.Join(", ", point.availableDirections)}");
    }

    void DetectIntersections()
    {
        var toMerge = new List<(RailPoint pA, RailPoint pB, Rail railB)>();
        var alreadyMerged = new HashSet<RailPoint>();

        for (int i = 0; i < rails.Count; i++)
            for (int j = i + 1; j < rails.Count; j++)
            {
                foreach (var pA in rails[i].points)
                    foreach (var pB in rails[j].points)
                    {
                        if (alreadyMerged.Contains(pB)) continue;
                        if (alreadyMerged.Contains(pA)) continue;

                        if (Vector2.Distance(pA.position, pB.position) < 0.1f)
                        {
                            toMerge.Add((pA, pB, rails[j]));
                            alreadyMerged.Add(pB);
                        }
                    }
            }

        foreach (var (pA, pB, railB) in toMerge)
            MergeIntersection(pA, pB, railB);
    }

    void MergeIntersection(RailPoint pA, RailPoint pB, Rail railB)
    {
        var snapshot = new Dictionary<Direction, RailPoint>(pB.connections);

        foreach (var kvp in snapshot)
        {
            // Only add if not already present
            if (!pA.connections.ContainsKey(kvp.Key))
                pA.connections[kvp.Key] = kvp.Value;

            kvp.Value.connections[RailUtils.Opposite(kvp.Key)] = pA;
        }

        foreach (var dir in pB.availableDirections)
            if (!pA.availableDirections.Contains(dir))
                pA.availableDirections.Add(dir);

        // Replace pB in all rails and connections
        foreach (var rail in rails)
        {
            for (int i = 0; i < rail.points.Count; i++)
                if (rail.points[i] == pB)
                    rail.points[i] = pA;

            foreach (var point in rail.points)
            {
                // Skip pA itself
                if (point == pA) continue;

                // Snapshot keys to avoid modifying while iterating
                var keys = new List<Direction>(point.connections.Keys);
                foreach (var key in keys)
                    if (point.connections[key] == pB)
                        point.connections[key] = pA;
            }
        }

        pA.type = ClassifyPoint(pA);
    }

    private RailPoint GetConnectPoint(Rail rail)
    {
        return rail.RailDirection == Direction.Left || rail.RailDirection == Direction.Up
            ? rail.LastPoint
            : rail.FirstPoint;
    }

    private void ApplyConnector()
    {
        foreach (var c in connectors)
        {
            if (c.fromRail == null || c.toRail == null)
            {
                Debug.LogWarning("Connector has missing rail reference, skipping.");
                continue;
            }

            RailPoint from = c.fromRail.LastPoint;
            RailPoint to   = GetConnectPoint(c.toRail);
            Direction back = RailUtils.Opposite(c.directionFromTo);
            
            from.connections[c.directionFromTo] = to;
            from.availableDirections.Add(c.directionFromTo);

            to.connections[back] = from;
            to.availableDirections.Add(back);

            from.type = ClassifyPoint(from);
            to.type = ClassifyPoint(to);
        }
    }

    PointType ClassifyPoint(RailPoint p)
    {
        int count = p.availableDirections.Count;
        if (count >= 3) return PointType.Junction;
        if (RailUtils.IsCorner(p.availableDirections)) return PointType.Corner;
        return PointType.Normal;
    }

    void OnDrawGizmos()
    {
        if (rails == null) return;

        foreach (var rail in rails)
        {
            if (rail.points == null) return;

            foreach (var point in rail.points)
            {
                // Color by type
                Gizmos.color = point.type switch
                {
                    PointType.Normal => Color.gray,
                    PointType.Junction => Color.yellow,
                    PointType.Corner => Color.blue,
                    _ => Color.white
                };

                Gizmos.DrawSphere(point.position, 0.15f);

                // Draw connections as lines
                Gizmos.color = Color.white;
                foreach (var connection in point.connections.Values)
                    Gizmos.DrawLine(point.position, connection.position);
            }
        }
    }
}
