using System.Collections.Generic;
using UnityEngine;

public enum CubeRequestType
{
    None,
    PickedTop,
    PickedBottom,
    PickedLeft,
    PickedRight,
    PickedFront,
    PickedBack,
    Released,
}

public class CubeRequest : MonoBehaviour
{
    public Queue<CubeRequestType> requests = new();

    private void OnEnable()
    {
        CubeSystem.requests.Add(this);
    }

    private void OnDisable()
    {
        CubeSystem.requests.Remove(this);
    }
}
