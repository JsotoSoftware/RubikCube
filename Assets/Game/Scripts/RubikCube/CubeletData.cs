using System.Collections.Generic;
using UnityEngine;

public class CubeletData 
{
    public Vector3Int position;
    public Transform cubelet;
    public Dictionary<Vector3, Vector3> faceDirections;

    private Dictionary<Vector3, Vector3> initialFaceDirections;

    public CubeletData(Vector3Int p, Transform t)
    {
        position = p;
        cubelet = t;
        faceDirections = new Dictionary<Vector3, Vector3>();
        initialFaceDirections = new Dictionary<Vector3, Vector3>();

        Vector3[] localDirections = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        foreach (Vector3 direction in localDirections)
        {
            var worldDirection = t.TransformDirection(direction).Round();
            faceDirections[direction] = worldDirection;
            initialFaceDirections[direction] = worldDirection;
        }
    }

    public void UpdateFaceDirection()
    {
        var keys = new List<Vector3>(faceDirections.Keys);
        foreach (Vector3 key in keys)
        {
            faceDirections[key] = cubelet.TransformDirection(key).Round();
        }
    }

    public bool IsInSolvedState ()
    {
        foreach (var key in faceDirections.Keys)
        {
            if (faceDirections[key] != initialFaceDirections[key])
            {
                return false;
            }
        }
        
        return true;
    }
}