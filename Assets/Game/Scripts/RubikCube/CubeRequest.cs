using System.Collections.Generic;
using UnityEngine;

public enum CubeFaceRequest
{
    None, PickedTop, PickedBottom, PickedLeft, PickedRight, PickedFront, PickedBack, Released
}

public enum CubeRotationRequest
{
    None, TopCW, TopCCW, BottomCW, BottomCCW, LeftCW, LeftCCW, RightCW, RightCCW, FrontCW, FrontCCW, BackCW, BackCCW
}

public enum CubeActionRequest
{
    None, Shuffle, Solve
}

public class CubeRequest : MonoBehaviour
{
    public Queue<object> requests = new();

    private void OnEnable()
    {
        CubeSystem.requests.Add(this);
    }

    private void OnDisable()
    {
        CubeSystem.requests.Remove(this);
    }
}
