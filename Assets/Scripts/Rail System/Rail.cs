using UnityEngine;
using System.Collections.Generic;

public class Rail : MonoBehaviour
{
    public Direction railDirection;
    public int pointCount = 10;
    public float spacing = 1f;

    public List<RailPoint> points = new ();

    public RailPoint FirstPoint => points[0];
    public RailPoint LastPoint => points[points.Count - 1];
    public Direction RailDirection => railDirection;

    public void GeneratePoint()
    {
        points.Clear();

        for (int i = 0; i < pointCount; i++)
        {
            var point = new RailPoint();
            point.position = railDirection switch
            {
                Direction.Right => (Vector2)transform.position + Vector2.right * (i * spacing),
                Direction.Left => (Vector2)transform.position + Vector2.left * (i * spacing),
                Direction.Up => (Vector2)transform.position + Vector2.up * (i * spacing),
                Direction.Down => (Vector2)transform.position + Vector2.down * (i * spacing),
                _ => transform.position
            };

            point.type = PointType.Normal;

            if (i > 0)
            {
                var prev = points[i - 1];
                Direction forward = railDirection;
                Direction back = RailUtils.Opposite(railDirection);

                // current point looks BACK to find previous
                point.connections[back] = prev;
                point.availableDirections.Add(back);

                // previous point looks FORWARD to find current
                prev.connections[forward] = point;
                prev.availableDirections.Add(forward);
            }

            points.Add(point);
        }
    }
}
