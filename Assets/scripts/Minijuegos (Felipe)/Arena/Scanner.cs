using System.Collections.Generic;
using UnityEngine;

public static class Scanner
{
    private const float DIRECTION_THRESHOLD = 0.15f;
    private const float MIN_SWIPE_DISTANCE = 10f;

    public static ShapeType Recognize(List<Vector2> rawPoints)
    {
        if (rawPoints == null || rawPoints.Count < 2)
            return ShapeType.Down;

        Vector2 start = rawPoints[0];
        Vector2 end = rawPoints[rawPoints.Count - 1];

        Vector2 delta = end - start;

        // evita micro movimientos accidentales
        if (delta.magnitude < MIN_SWIPE_DISTANCE)
            return ShapeType.Down;

        Vector2 direction = delta.normalized;

        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            return direction.y > 0
                ? ShapeType.Up
                : ShapeType.Down;
        }
        else
        {
            return direction.x > 0
                ? ShapeType.Right
                : ShapeType.Left;
        }
    }

}