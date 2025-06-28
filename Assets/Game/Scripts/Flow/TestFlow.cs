using UnityEngine;

public class TestFlow : MonoBehaviour
{
    [Header("---- References ----")]
    [SerializeField] private CubeRequest cubeRequest;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            cubeRequest.requests.Enqueue(CubeActionRequest.Shuffle);
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            cubeRequest.requests.Enqueue(CubeActionRequest.Solve);
        }
    }
}
