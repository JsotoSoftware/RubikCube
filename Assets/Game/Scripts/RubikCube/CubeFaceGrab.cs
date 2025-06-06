using UnityEngine;

[RequireComponent(typeof(CubeRequest))]
public class CubeFaceGrab : MonoBehaviour
{
    [Header("---- Settings ----")]
    [SerializeField] private CubeFaceRequest face;
    
    [Header("---- References ----")]
    [SerializeField] private CubeRequest request;
    
    [Header("---- Debug ----")]
    [SerializeField] private bool isGrabbed = false;
    [SerializeField] private Vector3 rotationAxis;

    private void Reset()
    {
        request = GetComponent<CubeRequest>();
    } 

    private void OnMouseDown()
    {
        isGrabbed = true;
        request.requests.Enqueue(face);
    }

    private void OnMouseUp()
    {
        isGrabbed = false;
        request.requests.Enqueue(CubeFaceRequest.Released);
    }

    private void Update ()
    {
        if(!isGrabbed)
            return;
    
        if(Input.GetKeyDown(KeyCode.A))
        {
            transform.Rotate(rotationAxis * 90);
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            transform.Rotate(rotationAxis * -90);
        }
    }
}
