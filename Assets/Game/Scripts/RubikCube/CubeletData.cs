using System.Collections.Generic;
using UnityEngine;
using Kociemba;

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

    public bool IsInSolvedState()
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

    public CubeColor GetFaceColor()
    {
        // Find the face that's pointing in the direction of its position
        foreach (var kvp in faceDirections)
        {
            Vector3 faceDir = kvp.Value;
            Vector3 posDir = ((Vector3)position).normalized;

            // If the face direction matches the position direction (within a small threshold)
            if (Vector3.Dot(faceDir, posDir) > 0.9f)
            {
                // Map the face direction to Kociemba color
                if (faceDir == Vector3.up) return CubeColor.U;
                if (faceDir == Vector3.down) return CubeColor.D;
                if (faceDir == Vector3.left) return CubeColor.L;
                if (faceDir == Vector3.right) return CubeColor.R;
                if (faceDir == Vector3.forward) return CubeColor.F;
                if (faceDir == Vector3.back) return CubeColor.B;
            }
        }

        // If no face is pointing in the position direction, find the face pointing up
        foreach (var kvp in faceDirections)
        {
            Vector3 faceDir = kvp.Value;
            if (faceDir == Vector3.up) return CubeColor.U;
            if (faceDir == Vector3.down) return CubeColor.D;
            if (faceDir == Vector3.left) return CubeColor.L;
            if (faceDir == Vector3.right) return CubeColor.R;
            if (faceDir == Vector3.forward) return CubeColor.F;
            if (faceDir == Vector3.back) return CubeColor.B;
        }

        return CubeColor.U; // Default fallback
    }
}