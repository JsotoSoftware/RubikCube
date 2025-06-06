using UnityEngine;

public class TestFlow : MonoBehaviour
{
    [Header("---- Settings ----")]
    [SerializeField] private int maxShuffleCount = 10;

    [Header("---- References ----")]
    [SerializeField] private CubeRequest cubeRequest;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            ShuffleCube(Random.Range(1, maxShuffleCount + 1));
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            // cubeRequest.requests.Enqueue(CubeFaceRequest.Released);
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
            bool isCounterClockwise = Random.Range(0, 2) == 1;

            // Randomly choose a face
            moves[i] = Random.Range(0, 6) switch
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
                cubeRequest.requests.Enqueue(CubeFaceRequest.Released);
                // Pick the new face
                SendFacePickRequest(currentFace);
            }
            else if(isFirstMove)
            {
                // Pick the face for the first move
                SendFacePickRequest(currentFace);
            }

            // Send rotation request
            SendRotationRequest(move);
            
            lastFace = currentFace;
            isFirstMove = false;
        }

        // Release the last face
        cubeRequest.requests.Enqueue(CubeFaceRequest.Released);
    }

    private void SendFacePickRequest(string face)
    {
        switch(face)
        {
            case "U":
                cubeRequest.requests.Enqueue(CubeFaceRequest.PickedTop);
                break;
            case "D":
                cubeRequest.requests.Enqueue(CubeFaceRequest.PickedBottom);
                break;
            case "L":
                cubeRequest.requests.Enqueue(CubeFaceRequest.PickedLeft);
                break;
            case "R":
                cubeRequest.requests.Enqueue(CubeFaceRequest.PickedRight);
                break;
            case "F":
                cubeRequest.requests.Enqueue(CubeFaceRequest.PickedFront);
                break;
            case "B":
                cubeRequest.requests.Enqueue(CubeFaceRequest.PickedBack);
                break;
        }
    }

    private void SendRotationRequest(string move)
    {
        bool isCounterClockwise = move.Contains("'");
        
        switch(move[0])
        {
            case 'U':
                cubeRequest.requests.Enqueue(isCounterClockwise ? CubeRotationRequest.TopCCW : CubeRotationRequest.TopCW);
                break;
            case 'D':
                cubeRequest.requests.Enqueue(isCounterClockwise ? CubeRotationRequest.BottomCCW : CubeRotationRequest.BottomCW);
                break;
            case 'L':
                cubeRequest.requests.Enqueue(isCounterClockwise ? CubeRotationRequest.LeftCCW : CubeRotationRequest.LeftCW);
                break;
            case 'R':
                cubeRequest.requests.Enqueue(isCounterClockwise ? CubeRotationRequest.RightCCW : CubeRotationRequest.RightCW);
                break;
            case 'F':
                cubeRequest.requests.Enqueue(isCounterClockwise ? CubeRotationRequest.FrontCCW : CubeRotationRequest.FrontCW);
                break;
            case 'B':
                cubeRequest.requests.Enqueue(isCounterClockwise ? CubeRotationRequest.BackCCW : CubeRotationRequest.BackCW);
                break;
        }
    }
}
