using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class FaceSelector : MonoBehaviour
{
    [Header("---- Settings ----")]
    [SerializeField] private float rotationSpeed = 1f;

    
    [Header("---- References ----")]
    [SerializeField] private InputActionReference inputPosition;
    [SerializeField] private InputActionReference inputClick;

    [Header("---- Internal Data ----")]
    private CubeFaceGrab selectedFace;
    private Vector2 lastPosition;
    private float initialAxisAngle;
    private Vector3 currentAxis;

    private void OnEnable()
    {
        inputPosition.action.Enable();
        inputClick.action.Enable();
        inputClick.action.performed += OnClick;
    }

    private void OnDisable()        
    {
        inputPosition.action.Disable();
        inputClick.action.Disable();
        inputClick.action.performed -= OnClick;
    }
    
    private void OnClick(InputAction.CallbackContext eventData)
    {
        if(eventData.control.IsPressed())
        {
            var screenPosition = inputPosition.action.ReadValue<Vector2>();

            var ray = Camera.main.ScreenPointToRay(screenPosition);
            if(Physics.Raycast(ray, out var hit))
            {
                if(hit.collider.TryGetComponent(out selectedFace))
                {
                    selectedFace.OnPress();
                    lastPosition = screenPosition;
                    currentAxis = selectedFace.rotationAxis;
                    initialAxisAngle = GetAxisAngle(selectedFace.transform, currentAxis);
                }   
            }
        }
        else
        {
            if(selectedFace != null)
            {
                ApplySnappedRotation(selectedFace.transform, currentAxis);
            }
        }
    }

    private void Update()
    {
        if(selectedFace != null)
        {
            var screenPosition = inputPosition.action.ReadValue<Vector2>();
            var delta = screenPosition - lastPosition;
            float movement = Mathf.Abs(selectedFace.rotationAxis.y + selectedFace.rotationAxis.z) > Mathf.Epsilon ? delta.x : delta.y;
            selectedFace.transform.Rotate(selectedFace.rotationAxis * movement * rotationSpeed * Time.deltaTime);

            lastPosition = screenPosition;
        }
    }

    private float GetAxisAngle(Transform t, Vector3 axis)
    {
        Vector3 euler = t.localEulerAngles;
        float angle = 0f;
        if(Mathf.Abs(axis.x) > 0.5f)
            angle = euler.x;
        else if(Mathf.Abs(axis.y) > 0.5f)
            angle = euler.y;
        else
            angle = euler.z;

        return Normalize180(angle);
    }

    private float Normalize180(float angle)
    {
        angle %= 360f;
        if(angle > 180f) angle -= 360f;
        if(angle < -180f) angle += 360f;
        return angle;
    }

    private void ApplySnappedRotation(Transform t, Vector3 axis)
    {
        float finalAngle = GetAxisAngle(selectedFace.transform, currentAxis);
        float deltaAngle = Normalize180(finalAngle - initialAxisAngle);

        int quarterTurns = Mathf.RoundToInt(deltaAngle / 90f);
        float targetAngle = initialAxisAngle + quarterTurns * 90f;

        Vector3 euler = t.localEulerAngles;
        if(Mathf.Abs(axis.x) > 0.5f)
            euler.x = targetAngle;
        else if(Mathf.Abs(axis.y) > 0.5f)
            euler.y = targetAngle;
        else
            euler.z = targetAngle;

        t.DOLocalRotate(euler, 0.5f).SetEase(Ease.OutBack).onComplete = () =>
        {
            if(quarterTurns != 0)
            {
                string direction = quarterTurns > 0 ? "positive" : "negative";
                Debug.Log($"Rotated {Mathf.Abs(quarterTurns)} quarter-turn(s) {direction} about axis {currentAxis}");
            }
            
            selectedFace.OnRelease();
            selectedFace = null;
        };
    }
}
