using System.Collections.Generic;
using UnityEngine;

public static class Scanner
{
    private const float DIRECTION_THRESHOLD = 0.6f;

    public static ShapeType Recognize(List<Vector2> rawPoints)
    {
        if (rawPoints == null || rawPoints.Count < 3)
            return ShapeType.Down;

        // Calcular dirección general del trazo
        Vector2 start = rawPoints[0];
        Vector2 end = rawPoints[rawPoints.Count - 1];
        Vector2 direction = (end - start).normalized;

        // Determinar dirección predominante
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            // Movimiento vertical
            return direction.y > DIRECTION_THRESHOLD ? ShapeType.Up : ShapeType.Down;
        }
        else
        {
            // Movimiento horizontal
            return direction.x > DIRECTION_THRESHOLD ? ShapeType.Right : ShapeType.Left;
        }
    }
}