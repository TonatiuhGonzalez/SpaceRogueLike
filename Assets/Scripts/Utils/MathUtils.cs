using UnityEngine;

public static class MathUtils
{
    public static Vector2 AngleToDirection(float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }
}
