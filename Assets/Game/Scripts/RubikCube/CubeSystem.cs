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

    private void Update()
    {
        foreach (var request in requests)
        {
            if (request.requests.Count > 0)
            {
                var requestType = request.requests.Dequeue();
                
                ProcessRequest(requestType);
            }
        }
    }

    private void ProcessRequest(CubeRequestType requestType)
    {
        if(requestType == CubeRequestType.PickedTop )
        {
            // Join all cubelets in the top face
            foreach (var cubelet in cubelets)
            {
                if(cubelet.localPosition.y > 0) 
                {
                    cubelet.SetParent(topFaceParent);
                }
            }
        }
        else if(requestType == CubeRequestType.PickedBottom)
        {
            // Join all cubelets in the bottom face
            foreach (var cubelet in cubelets)
            {
                if(cubelet.localPosition.y < 0) 
                {
                    cubelet.SetParent(bottomFaceParent);
                }
            }   
        }
        else if(requestType == CubeRequestType.PickedLeft)
        {
            // Join all cubelets in the left face
            foreach (var cubelet in cubelets)
            {
                if(cubelet.localPosition.x > 0) 
                {
                    cubelet.SetParent(leftFaceParent);
                }
            }
        }
        else if(requestType == CubeRequestType.PickedRight)
        {
            // Join all cubelets in the right face
            foreach (var cubelet in cubelets)
            {
                if(cubelet.localPosition.x < 0) 
                {
                    cubelet.SetParent(rightFaceParent);
                }
            }
        }
        else if(requestType == CubeRequestType.PickedFront)
        {
            // Join all cubelets in the front face
            foreach (var cubelet in cubelets)
            {
                if(cubelet.localPosition.z > 0) 
                {
                    cubelet.SetParent(frontFaceParent);
                }
            }
        }
        else if(requestType == CubeRequestType.PickedBack)
        {
            // Join all cubelets in the back face           
            foreach (var cubelet in cubelets)
            {
                if(cubelet.localPosition.z < 0) 
                {
                    cubelet.SetParent(backFaceParent);
                }
            }
        }
        else if(requestType == CubeRequestType.Released)
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
        }
    }
}
