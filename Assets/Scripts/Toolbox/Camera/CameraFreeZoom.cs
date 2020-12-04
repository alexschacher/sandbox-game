using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class CameraFreeZoom : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] [Range(0f, 1f)] private float zoom = 0f;
    [SerializeField] private Transform zoomedInPosition;
    [SerializeField] private Transform zoomedOutPosition;
    private Vector3 targetZoomPosition;

    [Header("Input")]
    [SerializeField] private string inputAxis = "Vertical";
    [SerializeField] private bool canControl = true;
    [SerializeField] private bool invertInput;

    [Header("Motion")]
    [SerializeField] [Range(0.5f, 10f)] private float speed = 1f;
    [SerializeField] [Range(0f, 3f)] private float lag = 1f;
    private Vector3 zoomVelocity = Vector3.zero;

    private CameraController cameraController;
    private Transform cam;

    private void Start()
    {
        cameraController = GetComponent<CameraController>();
        cam = cameraController.Cam;
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

        if (input < 0f && zoom > 0f)
        {
            zoom -= speed * Time.deltaTime;
            if (zoom < 0f)
            {
                zoom = 0f;
            }
        }
        else if (input > 0f && zoom < 1f)
        {
            zoom += speed * Time.deltaTime;
            if (zoom > 1f)
            {
                zoom = 1f;
            }
        }
    }

    private void Lerp()
    {
        if (cam == null || zoomedInPosition == null || zoomedOutPosition == null) return;

        targetZoomPosition = new Vector3(
            Mathf.Lerp(zoomedOutPosition.localPosition.x, zoomedInPosition.localPosition.x, zoom),
            Mathf.Lerp(zoomedOutPosition.localPosition.y, zoomedInPosition.localPosition.y, zoom),
            Mathf.Lerp(zoomedOutPosition.localPosition.z, zoomedInPosition.localPosition.z, zoom));

        cam.localPosition = Vector3.SmoothDamp(
            cam.localPosition,
            targetZoomPosition,
            ref zoomVelocity,
            lag / 10
            );
    }
}