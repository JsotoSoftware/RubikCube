using UnityEngine;
using System.Collections.Generic;
using System;

public class CubeSystem : MonoBehaviour
{
    [Header("---- Settings ----")]
    [SerializeField] private float rotationSpeed = 10f;
    
    [Header("---- References ----")]
    [SerializeField] private Transform[] cubelets;
    [SerializeField] private Transform topFaceParent;
    [SerializeField] private Transform bottomFaceParent;
    [SerializeField] private Transform leftFaceParent;
    [SerializeField] private Transform rightFaceParent;
    [SerializeField] private Transform frontFaceParent;
    [SerializeField] private Transform backFaceParent;


    [Header("---- Data ----")]
    private Dictionary<Vector3Int, CubeletData> cubeState = new();

    public static List<CubeRequest> requests = new();

    private void Start()
    {
        cubeState = new Dictionary<Vector3Int, CubeletData>();
        foreach (Transform cubelet in cubelets)
        {
            var position = cubelet.position.RoundVector3Int();
            cubeState.Add(position, new CubeletData(position, cubelet));
        }
    }

    private void UpdateCubeState()
    {
        var newCubeState = new Dictionary<Vector3Int, CubeletData>();
        foreach (var cubeletData in cubeState.Values)
        {
            Vector3Int newPos = cubeletData.cubelet.localPosition.RoundVector3Int();
            cubeletData.position = newPos;
            cubeletData.UpdateFaceDirection();
            newCubeState.Add(newPos, cubeletData);
        }
        
        cubeState = newCubeState;
    }

    private bool IsItSolved ()
    {
        foreach (var cubeletData in cubeState.Values)
        {
            if (!cubeletData.IsInSolvedState())
                return false;
        }
        return true;
    }

    private void Update()
    {
        foreach (var request in requests)
        {
            if (request.requests.Count > 0)
            {
                var requestType = request.requests.Dequeue();
                
                if(requestType is CubeFaceRequest)
                    JoinFaces((CubeFaceRequest) requestType);
                else if(requestType is CubeRotationRequest)
                    RotateFaces((CubeRotationRequest) requestType);
            }
        }
    }

    private void RotateFaces(CubeRotationRequest requestType)
    {
        switch(requestType)
        {
            case CubeRotationRequest.TopCW:
                topFaceParent.Rotate(Vector3.up, 90);
                break;
            case CubeRotationRequest.TopCCW:
                topFaceParent.Rotate(Vector3.up, -90);
                break;
            case CubeRotationRequest.BottomCW:
                bottomFaceParent.Rotate(Vector3.up, 90);
                break;
            case CubeRotationRequest.BottomCCW:
                bottomFaceParent.Rotate(Vector3.up, -90);
                break;
            case CubeRotationRequest.LeftCW:
                leftFaceParent.Rotate(Vector3.right, -90);
                break;
            case CubeRotationRequest.LeftCCW:
                leftFaceParent.Rotate(Vector3.right, 90);
                break;
            case CubeRotationRequest.RightCW:
                rightFaceParent.Rotate(Vector3.right, -90);
                break;
            case CubeRotationRequest.RightCCW:
                rightFaceParent.Rotate(Vector3.right, 90);
                break;
            case CubeRotationRequest.FrontCW:
                frontFaceParent.Rotate(Vector3.forward, 90);
                break;
            case CubeRotationRequest.FrontCCW:
                frontFaceParent.Rotate(Vector3.forward, -90);
                break;
            case CubeRotationRequest.BackCW:
                backFaceParent.Rotate(Vector3.back, 90);
                break;
            case CubeRotationRequest.BackCCW:
                backFaceParent.Rotate(Vector3.back, -90);
                break;
        }
    }

    private void JoinFaces(CubeFaceRequest requestType)
    {
        if(requestType == CubeFaceRequest.PickedTop )
        {
            // Join all cubelets in the top face
            foreach (var cubelet in cubelets)
            {
                if(cubelet.localPosition.y > 0) 
                    cubelet.SetParent(topFaceParent);
            }
        }
        else if(requestType == CubeFaceRequest.PickedBottom)
        {
            // Join all cubelets in the bottom face
            foreach (var cubelet in cubelets)
            {
                if(cubelet.localPosition.y < 0) 
                    cubelet.SetParent(bottomFaceParent);
            }   
        }
        else if(requestType == CubeFaceRequest.PickedLeft)
        {
            // Join all cubelets in the left face
            foreach (var cubelet in cubelets)
            {
                if(cubelet.localPosition.x > 0) 
                    cubelet.SetParent(leftFaceParent);
            }
        }
        else if(requestType == CubeFaceRequest.PickedRight)
        {
            // Join all cubelets in the right face
            foreach (var cubelet in cubelets)
            {
                if(cubelet.localPosition.x < 0) 
                    cubelet.SetParent(rightFaceParent);
            }
        }
        else if(requestType == CubeFaceRequest.PickedFront)
        {
            // Join all cubelets in the front face
            foreach (var cubelet in cubelets)
            {
                if(cubelet.localPosition.z > 0) 
                    cubelet.SetParent(frontFaceParent);
            }
        }
        else if(requestType == CubeFaceRequest.PickedBack)
        {
            // Join all cubelets in the back face           
            foreach (var cubelet in cubelets)
            {
                if(cubelet.localPosition.z < 0) 
                    cubelet.SetParent(backFaceParent);
            }
        }
        else if(requestType == CubeFaceRequest.Released)
        {
            foreach (var cubelet in cubelets)
            {
                if(cubelet.parent != transform) 
                {
                    cubelet.SetParent(transform);
                    cubelet.localPosition = cubelet.localPosition.Round();
                }
            }

            UpdateCubeState();

            // Debug.Log("Is it solved? " + IsItSolved());
        }
    }
}
