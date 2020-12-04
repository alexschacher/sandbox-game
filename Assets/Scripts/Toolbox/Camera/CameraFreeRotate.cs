using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class CameraFreeRotate : MonoBehaviour
{
    [Header("Angle")]
    [SerializeField] [Range(0f, 360f)] private float targetAngle = 45f;
    private Vector3 targetRotation;

    [Header("Input")]
    [SerializeField] private string inputAxis = "Horizontal";
    [SerializeField] private bool canControl = true;
    [SerializeField] private bool invertInput;

    [Header("Motion")]
    [SerializeField] [Range(0.5f, 3f)] private float rotateSpeed = 1f;
    [SerializeField] [Range(0.5f, 3f)] private float snappiness = 1f;

    private void Start()
    {
        targetRotation = transform.eulerAngles;
    }

    private void Update()
    {
        if (canControl)
        {
            GetInput();
        }
        Lerp();
    }

    private void GetInput()
    {
        float input = Input.GetAxisRaw(inputAxis);
        if (invertInput)
        {
            input = -input;
        }

        if (input < 0f)
        {
            targetAngle = targetRotation.y + rotateSpeed * 200 * Time.deltaTime;
            if (targetAngle > 360)
            {
                targetAngle -= 360;
            }
        }
        else if (input > 0f)
        {
            targetAngle = targetRotation.y - rotateSpeed * 200 * Time.deltaTime;
            if (targetAngle < 0)
            {
                targetAngle += 360;
            }
        }
    }

    private void Lerp()
    {
        targetRotation = new Vector3(targetRotation.x, targetAngle, targetRotation.z);

        transform.eulerAngles = new Vector3(
            Mathf.LerpAngle(transform.eulerAngles.x, targetRotation.x, Time.deltaTime * snappiness * 10),
            Mathf.LerpAngle(transform.eulerAngles.y, targetRotation.y, Time.deltaTime * snappiness * 10),
            Mathf.LerpAngle(transform.eulerAngles.z, targetRotation.z, Time.deltaTime * snappiness * 10));
    }
}