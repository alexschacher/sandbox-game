using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class CameraSnapZoom : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] [Range(0f, 1f)] private float zoom = 0f;
    [SerializeField] [Range(2, 5)] private int zoomLevels = 3;
    [SerializeField] private Transform zoomedInPosition;
    [SerializeField] private Transform zoomedOutPosition;
    private Vector3 targetZoomPosition;

    [Header("Input")]
    [SerializeField] private string inputAxis = "Vertical";
    [SerializeField] private bool canControl = true;
    [SerializeField] private bool invertInput;
    private bool canTakeInput;

    [Header("Motion")]
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
        float zoomIncrement = 1f / (zoomLevels - 1);
        float input = Input.GetAxisRaw(inputAxis);
        if (invertInput)
        {
            input = -input;
        }

        if (input == 0f)
        {
            canTakeInput = true;
        }
        else if (input < 0f && zoom > 0f && canTakeInput)
        {
            canTakeInput = false;
            zoom -= zoomIncrement;
            if (zoom < 0.09f)
            {
                zoom = 0f;
            }
        }
        else if (input > 0f && zoom < 1f && canTakeInput)
        {
            canTakeInput = false;
            zoom += zoomIncrement;
            if (zoom > 0.91f)
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