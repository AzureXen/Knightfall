using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Returns the normalized direction vector pointing from this origin to the target.
    /// Usage: Vector2 dir = transform.DirectionTo(player.transform);
    /// </summary>
    public static Vector2 DirectionTo(this Transform origin, Transform target)
    {
        if (origin == null || target == null) return Vector2.zero;
        return (target.position - origin.position).normalized;
    }

    /// <summary>
    /// Overload for GameObject targets.
    /// Usage: Vector2 dir = transform.DirectionTo(player);
    /// </summary>
    public static Vector2 DirectionTo(this Transform origin, GameObject target)
    {
        if (origin == null || target == null) return Vector2.zero;
        return (target.transform.position - origin.position).normalized;
    }

    /// <summary>
    /// Returns normalized direction from Vector3 position to Vector3 target.
    /// Usage: Vector2 dir = transform.position.DirectionTo(player.transform.position);
    /// </summary>
    public static Vector2 DirectionTo(this Vector3 origin, Vector3 target)
    {
        return (target - origin).normalized;
    }

    /// <summary>
    /// Returns 2D distance between origin and target.
    /// Usage: float dist = transform.DistanceTo(player.transform);
    /// </summary>
    public static float DistanceTo(this Transform origin, Transform target)
    {
        if (origin == null || target == null) return 0f;
        return Vector2.Distance(origin.position, target.position);
    }
}
