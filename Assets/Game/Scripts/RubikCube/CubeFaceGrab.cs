using UnityEngine;

[RequireComponent(typeof(CubeRequest))]
public class CubeFaceGrab : MonoBehaviour
{
    [Header("---- Settings ----")]
    public Vector3 rotationAxis;
    [SerializeField] private CubeFaceRequest face;
    
    [Header("---- References ----")]
    [SerializeField] private CubeRequest request;
    
    private void Reset()
    {
        request = GetComponent<CubeRequest>();
    } 

    public void OnPress()
    {
        Debug.Log("OnMouseDown");
        request.requests.Enqueue(face);
    }

    public void OnRelease()
    {
        Debug.Log("OnMouseUp");
        request.requests.Enqueue(CubeFaceRequest.Released);
    }
}
