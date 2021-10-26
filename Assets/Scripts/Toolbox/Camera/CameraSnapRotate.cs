using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class CameraSnapRotate : MonoBehaviour
{
    [Header("Angle")]
    [SerializeField] [Range(0f, 360f)] private float targetAngle = 45f;
    [SerializeField] [Range(0f, 180f)] private float angleIncrement = 45f;
    private Vector3 targetRotation;

    [Header("Input")]
    [SerializeField] private bool canControl = true;
    [SerializeField] private bool invertInput;

    [Header("Motion")]
    [SerializeField] [Range(0.5f, 3f)] private float snappiness = 1f;

    private void Awake()
    {
        targetRotation = transform.eulerAngles;
    }
    private void Update()
    {
        Lerp();
    }
    private void Lerp()
    {
        targetRotation = new Vector3(targetRotation.x, targetAngle, targetRotation.z);

        transform.eulerAngles = new Vector3(
            Mathf.LerpAngle(transform.eulerAngles.x, targetRotation.x, Time.deltaTime * snappiness * 10),
            Mathf.LerpAngle(transform.eulerAngles.y, targetRotation.y, Time.deltaTime * snappiness * 10),
            Mathf.LerpAngle(transform.eulerAngles.z, targetRotation.z, Time.deltaTime * snappiness * 10));
    }
    private void OnRotateCameraLeft() { if (!App.IsInputLocked()) { RotateCamera(true); } }
    private void OnRotateCameraRight() { if (!App.IsInputLocked()) { RotateCamera(false); } }
    private void RotateCamera(bool left)
    {
        if (!canControl) return;
        if (invertInput) left = !left;

        if (left)
        {
            targetAngle = targetRotation.y + angleIncrement;
            if (targetAngle > 360)
            {
                targetAngle -= 360;
            }
        }
        else
        {
            targetAngle = targetRotation.y - angleIncrement;
            if (targetAngle < 0)
            {
                targetAngle += 360;
            }
        }
    }
    
}