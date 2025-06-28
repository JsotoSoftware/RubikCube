using UnityEngine;

public static class CubeExtensions
{
    public static void RoundLocalPosition(this Transform transform)
    {
        transform.localPosition = new Vector3(
            Mathf.RoundToInt(transform.localPosition.x),
            Mathf.RoundToInt(transform.localPosition.y),
            Mathf.RoundToInt(transform.localPosition.z)
        );
    }
}