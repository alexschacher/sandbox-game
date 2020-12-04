using UnityEngine;

// Camera Target (include scripts here) (pivot rotation here)
// - Main Camera (transform should be default, except use negative Z position to increase camera distance)
// - Zoomed In Position
// - Zoomed Out Position

// TODO: CameraAutoRotate (For fancy 3d model displays)
// TODO: CameraPanWithMouse

public class CameraController : MonoBehaviour
{
    [HideInInspector] public static CameraController Instance { get; private set; }
    [HideInInspector] public Transform Cam { get; private set; }

    [SerializeField] private bool alwaysFaceTarget = true;

    private void Awake()
    {
        Instance = (Instance == null) ? this : Instance;
        Cam = (Cam == null) ? Camera.main.transform : Cam;
    }

    private void Update()
    {
        if (alwaysFaceTarget)
        {
            Cam.LookAt(transform.position);
        }
    }
}