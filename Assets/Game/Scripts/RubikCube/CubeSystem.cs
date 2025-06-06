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
    private Dictionary<Vector3Int, CubeletData> cubeState = new();

    public static List<CubeRequest> requests = new();
    private int maxShuffleCount = 10;


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
                else if(requestType is CubeActionRequest)
                    HandleAction((CubeActionRequest) requestType);
            }
        }
    }

    private void HandleAction(CubeActionRequest requestType)
    {
        if(requestType == CubeActionRequest.Shuffle)
        {
            // Shuffle the cube
            ShuffleCube(UnityEngine.Random.Range(1, maxShuffleCount + 1));
        }
        else if(requestType == CubeActionRequest.Solve)
        {
            // Solve the cube
            Debug.Log("Solving cube: " + GetKociembaString());
        }
    }

    private void ShuffleCube(int shuffleTimes)
    {
        // Make a random list of moves
        Debug.Log($"Shuffling cube {shuffleTimes} times");
        string[] moves = new string[shuffleTimes];
        for(int i = 0; i < shuffleTimes; i++)
        {
            // Randomly choose a rotation
            bool isCounterClockwise = UnityEngine.Random.Range(0, 2) == 1;

            // Randomly choose a face
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
            Debug.Log($"Sending move: {move}");

            string currentFace = move[0].ToString();
            
            // If we're changing faces and it's not the first move, release the current face
            if(!isFirstMove && currentFace != lastFace)
            {
                JoinFaces(CubeFaceRequest.Released);
                SendFacePickRequest(currentFace);
            }
            else if(isFirstMove)
            {
                SendFacePickRequest(currentFace);
            }

            SendRotationRequest(move);
            
            lastFace = currentFace;
            isFirstMove = false;
        }

        JoinFaces(CubeFaceRequest.Released);
    }

    private void SendFacePickRequest(string face)
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

    private void SendRotationRequest(string move)
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

    public string GetKociembaString()
    {
        char[] kociembaString = new char[54];
        
        // Helper function to get color at a specific position
        CubeColor GetColorAtPosition(Vector3 position)
        {
            // Find the cubelet at this position
            foreach (var cubeletData in cubeState.Values)
            {
                if (cubeletData.position == position.RoundVector3Int())
                {
                    // Get the color based on the face direction
                    return cubeletData.GetFaceColor();
                }
            }
            return CubeColor.U; // Default fallback
        }

        // Map positions to Kociemba facelet indices
        // U face (y = 1)
        kociembaString[0] = GetColorAtPosition(new Vector3(-1, 1, 1)).ToString()[0];  // U1
        kociembaString[1] = GetColorAtPosition(new Vector3(0, 1, 1)).ToString()[0];   // U2
        kociembaString[2] = GetColorAtPosition(new Vector3(1, 1, 1)).ToString()[0];   // U3
        kociembaString[3] = GetColorAtPosition(new Vector3(-1, 1, 0)).ToString()[0];  // U4
        kociembaString[4] = GetColorAtPosition(new Vector3(0, 1, 0)).ToString()[0];   // U5
        kociembaString[5] = GetColorAtPosition(new Vector3(1, 1, 0)).ToString()[0];   // U6
        kociembaString[6] = GetColorAtPosition(new Vector3(-1, 1, -1)).ToString()[0]; // U7
        kociembaString[7] = GetColorAtPosition(new Vector3(0, 1, -1)).ToString()[0];  // U8
        kociembaString[8] = GetColorAtPosition(new Vector3(1, 1, -1)).ToString()[0];  // U9

        // R face (x = 1)
        kociembaString[9] = GetColorAtPosition(new Vector3(1, 1, 1)).ToString()[0];   // R1
        kociembaString[10] = GetColorAtPosition(new Vector3(1, 1, 0)).ToString()[0];  // R2
        kociembaString[11] = GetColorAtPosition(new Vector3(1, 1, -1)).ToString()[0]; // R3
        kociembaString[12] = GetColorAtPosition(new Vector3(1, 0, 1)).ToString()[0];  // R4
        kociembaString[13] = GetColorAtPosition(new Vector3(1, 0, 0)).ToString()[0];  // R5
        kociembaString[14] = GetColorAtPosition(new Vector3(1, 0, -1)).ToString()[0]; // R6
        kociembaString[15] = GetColorAtPosition(new Vector3(1, -1, 1)).ToString()[0]; // R7
        kociembaString[16] = GetColorAtPosition(new Vector3(1, -1, 0)).ToString()[0]; // R8
        kociembaString[17] = GetColorAtPosition(new Vector3(1, -1, -1)).ToString()[0];// R9

        // F face (z = 1)
        kociembaString[18] = GetColorAtPosition(new Vector3(1, 1, 1)).ToString()[0];  // F1
        kociembaString[19] = GetColorAtPosition(new Vector3(0, 1, 1)).ToString()[0];  // F2
        kociembaString[20] = GetColorAtPosition(new Vector3(-1, 1, 1)).ToString()[0]; // F3
        kociembaString[21] = GetColorAtPosition(new Vector3(1, 0, 1)).ToString()[0];  // F4
        kociembaString[22] = GetColorAtPosition(new Vector3(0, 0, 1)).ToString()[0];  // F5
        kociembaString[23] = GetColorAtPosition(new Vector3(-1, 0, 1)).ToString()[0]; // F6
        kociembaString[24] = GetColorAtPosition(new Vector3(1, -1, 1)).ToString()[0]; // F7
        kociembaString[25] = GetColorAtPosition(new Vector3(0, -1, 1)).ToString()[0]; // F8
        kociembaString[26] = GetColorAtPosition(new Vector3(-1, -1, 1)).ToString()[0];// F9

        // D face (y = -1)
        kociembaString[27] = GetColorAtPosition(new Vector3(-1, -1, 1)).ToString()[0];// D1
        kociembaString[28] = GetColorAtPosition(new Vector3(0, -1, 1)).ToString()[0]; // D2
        kociembaString[29] = GetColorAtPosition(new Vector3(1, -1, 1)).ToString()[0]; // D3
        kociembaString[30] = GetColorAtPosition(new Vector3(-1, -1, 0)).ToString()[0];// D4
        kociembaString[31] = GetColorAtPosition(new Vector3(0, -1, 0)).ToString()[0]; // D5
        kociembaString[32] = GetColorAtPosition(new Vector3(1, -1, 0)).ToString()[0]; // D6
        kociembaString[33] = GetColorAtPosition(new Vector3(-1, -1, -1)).ToString()[0];// D7
        kociembaString[34] = GetColorAtPosition(new Vector3(0, -1, -1)).ToString()[0];// D8
        kociembaString[35] = GetColorAtPosition(new Vector3(1, -1, -1)).ToString()[0];// D9

        // L face (x = -1)
        kociembaString[36] = GetColorAtPosition(new Vector3(-1, 1, -1)).ToString()[0];// L1
        kociembaString[37] = GetColorAtPosition(new Vector3(-1, 1, 0)).ToString()[0]; // L2
        kociembaString[38] = GetColorAtPosition(new Vector3(-1, 1, 1)).ToString()[0]; // L3
        kociembaString[39] = GetColorAtPosition(new Vector3(-1, 0, -1)).ToString()[0];// L4
        kociembaString[40] = GetColorAtPosition(new Vector3(-1, 0, 0)).ToString()[0]; // L5
        kociembaString[41] = GetColorAtPosition(new Vector3(-1, 0, 1)).ToString()[0]; // L6
        kociembaString[42] = GetColorAtPosition(new Vector3(-1, -1, -1)).ToString()[0];// L7
        kociembaString[43] = GetColorAtPosition(new Vector3(-1, -1, 0)).ToString()[0];// L8
        kociembaString[44] = GetColorAtPosition(new Vector3(-1, -1, 1)).ToString()[0];// L9

        // B face (z = -1)
        kociembaString[45] = GetColorAtPosition(new Vector3(-1, 1, -1)).ToString()[0];// B1
        kociembaString[46] = GetColorAtPosition(new Vector3(0, 1, -1)).ToString()[0]; // B2
        kociembaString[47] = GetColorAtPosition(new Vector3(1, 1, -1)).ToString()[0]; // B3
        kociembaString[48] = GetColorAtPosition(new Vector3(-1, 0, -1)).ToString()[0];// B4
        kociembaString[49] = GetColorAtPosition(new Vector3(0, 0, -1)).ToString()[0]; // B5
        kociembaString[50] = GetColorAtPosition(new Vector3(1, 0, -1)).ToString()[0]; // B6
        kociembaString[51] = GetColorAtPosition(new Vector3(-1, -1, -1)).ToString()[0];// B7
        kociembaString[52] = GetColorAtPosition(new Vector3(0, -1, -1)).ToString()[0];// B8
        kociembaString[53] = GetColorAtPosition(new Vector3(1, -1, -1)).ToString()[0];// B9

        return new string(kociembaString);
    }
}
