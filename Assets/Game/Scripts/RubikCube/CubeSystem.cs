using UnityEngine;
using System.Collections.Generic;
using System;
using Kociemba;

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
    public static List<CubeRequest> requests = new();
    private int maxShuffleCount = 10;

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
                else if(requestType is CubeActionRequest)
                    HandleAction((CubeActionRequest) requestType);
            }
        }
    }

    private void HandleAction(CubeActionRequest requestType)
    {
        if(requestType == CubeActionRequest.Shuffle)
        {
            ShuffleCube(UnityEngine.Random.Range(1, maxShuffleCount + 1));
        }
        else if(requestType == CubeActionRequest.Solve)
        {

        }
    }

    private void ShuffleCube(int shuffleTimes)
    {
        string[] moves = new string[shuffleTimes];
        for(int i = 0; i < shuffleTimes; i++)
        {
            bool isCounterClockwise = UnityEngine.Random.Range(0, 2) == 1;

            moves[i] = UnityEngine.Random.Range(0, 6) switch
            {
                0 => isCounterClockwise ? "U'" : "U",
                1 => isCounterClockwise ? "D'" : "D",
                2 => isCounterClockwise ? "L'" : "L",
                3 => isCounterClockwise ? "R'" : "R",
                4 => isCounterClockwise ? "F'" : "F",
                5 => isCounterClockwise ? "B'" : "B",
                _ => throw new System.Exception("Invalid move index")
            };
        }

        string lastFace = "";
        bool isFirstMove = true;

        foreach(var move in moves)
        {
            string currentFace = move[0].ToString();
            
            if(!isFirstMove && currentFace != lastFace)
            {
                JoinFaces(CubeFaceRequest.Released);
                SetFacePick(currentFace);
            }
            else if(isFirstMove)
            {
                SetFacePick(currentFace);
            }

            SetRotation(move);
            
            lastFace = currentFace;
            isFirstMove = false;
        }

        JoinFaces(CubeFaceRequest.Released);
    }

    private void SetFacePick(string face)
    {
        switch(face)
        {
            case "U":
                JoinFaces(CubeFaceRequest.PickedTop);
                break;
            case "D":
                JoinFaces(CubeFaceRequest.PickedBottom);
                break;
            case "L":
                JoinFaces(CubeFaceRequest.PickedLeft);
                break;
            case "R":
                JoinFaces(CubeFaceRequest.PickedRight);
                break;
            case "F":
                JoinFaces(CubeFaceRequest.PickedFront);
                break;
            case "B":
                JoinFaces(CubeFaceRequest.PickedBack);
                break;
        }
    }

    private void SetRotation(string move)
    {
        bool isCounterClockwise = move.Contains("'");
        
        switch(move[0])
        {
            case 'U':
                RotateFaces(isCounterClockwise ? CubeRotationRequest.TopCCW : CubeRotationRequest.TopCW);
                break;
            case 'D':
                RotateFaces(isCounterClockwise ? CubeRotationRequest.BottomCCW : CubeRotationRequest.BottomCW);
                break;
            case 'L':
                RotateFaces(isCounterClockwise ? CubeRotationRequest.LeftCCW : CubeRotationRequest.LeftCW);
                break;
            case 'R':
                RotateFaces(isCounterClockwise ? CubeRotationRequest.RightCCW : CubeRotationRequest.RightCW);
                break;
            case 'F':
                RotateFaces(isCounterClockwise ? CubeRotationRequest.FrontCCW : CubeRotationRequest.FrontCW);
                break;
            case 'B':
                RotateFaces(isCounterClockwise ? CubeRotationRequest.BackCCW : CubeRotationRequest.BackCW);
                break;
        }
    }

    private void RotateFaces(CubeRotationRequest requestType)
    {
        Transform faceParent = null;
        Vector3 axis = Vector3.zero;
        float angle = 0f;

        switch(requestType)
        {
            case CubeRotationRequest.TopCW:
                faceParent = topFaceParent;
                axis = Vector3.up;
                angle = 90f;
                break;
            case CubeRotationRequest.TopCCW:
                faceParent = topFaceParent;
                axis = Vector3.up;
                angle = -90f;
                break;
            case CubeRotationRequest.BottomCW:
                faceParent = bottomFaceParent;
                axis = Vector3.up;
                angle = -90f;
                break;
            case CubeRotationRequest.BottomCCW:
                faceParent = bottomFaceParent;
                axis = Vector3.up;
                angle = 90f;
                break;
            case CubeRotationRequest.LeftCW:
                faceParent = leftFaceParent;
                axis = Vector3.right;
                angle = -90f;       
                break;
            case CubeRotationRequest.LeftCCW:
                faceParent = leftFaceParent;
                axis = Vector3.right;
                angle = 90f;
                break;
            case CubeRotationRequest.RightCW:
                faceParent = rightFaceParent;
                axis = Vector3.right;
                angle = 90f;
                break;
            case CubeRotationRequest.RightCCW:
                faceParent = rightFaceParent;
                axis = Vector3.right;
                angle = -90f;
                break;
            case CubeRotationRequest.FrontCW:
                faceParent = frontFaceParent;
                axis = Vector3.forward;
                angle = -90f;
                break;
            case CubeRotationRequest.FrontCCW:
                faceParent = frontFaceParent;
                axis = Vector3.forward;
                angle = 90f;   
                break;
            case CubeRotationRequest.BackCW:
                faceParent = backFaceParent;
                axis = Vector3.back;
                angle = 90f;
                break;
            case CubeRotationRequest.BackCCW:
                faceParent = backFaceParent;
                axis = Vector3.back;
                angle = -90f;
                break;
        }

        if (faceParent != null)
        {
            faceParent.Rotate(axis, angle);
            faceParent.RoundLocalPosition();
            foreach(Transform cubelet in faceParent)
            {
                cubelet.RoundLocalPosition();
            }
        }
    }

    private void JoinFaces(CubeFaceRequest requestType)
    {
        if(requestType == CubeFaceRequest.PickedTop )
        {
            foreach (var cubelet in cubelets)
            {
                if(Mathf.Round(cubelet.localPosition.y) > 0) 
                    cubelet.SetParent(topFaceParent);
            }
        }
        else if(requestType == CubeFaceRequest.PickedBottom)
        {
            foreach (var cubelet in cubelets)
            {
                if(Mathf.Round(cubelet.localPosition.y) < 0) 
                    cubelet.SetParent(bottomFaceParent);
            }   
        }
        else if(requestType == CubeFaceRequest.PickedLeft)
        {
            foreach (var cubelet in cubelets)
            {
                if(Mathf.Round(cubelet.localPosition.x) > 0) 
                    cubelet.SetParent(leftFaceParent);
            }
        }
        else if(requestType == CubeFaceRequest.PickedRight)
        {
            // Join all cubelets in the right face
            foreach (var cubelet in cubelets)
            {
                if(Mathf.Round(cubelet.localPosition.x) < 0) 
                    cubelet.SetParent(rightFaceParent);
            }
        }
        else if(requestType == CubeFaceRequest.PickedFront)
        {
            // Join all cubelets in the front face
            foreach (var cubelet in cubelets)
            {
                if(Mathf.Round(cubelet.localPosition.z) < 0) 
                    cubelet.SetParent(frontFaceParent);
            }
        }
        else if(requestType == CubeFaceRequest.PickedBack)
        {
            // Join all cubelets in the back face           
            foreach (var cubelet in cubelets)
            {
                if(Mathf.Round(cubelet.localPosition.z) > 0) 
                    cubelet.SetParent(backFaceParent);
            }
        }
        else if(requestType == CubeFaceRequest.Released)
        {
            foreach (var cubelet in cubelets)
            {
                if(cubelet.parent != transform) 
                {
                    Quaternion worldRot = cubelet.rotation;
                    cubelet.SetParent(transform, true);
                    cubelet.rotation = worldRot;
                }
            }
        }
    }
}
