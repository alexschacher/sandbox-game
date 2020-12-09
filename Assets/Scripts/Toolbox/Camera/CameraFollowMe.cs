using UnityEngine;
using Mirror;

public class CameraFollowMe : NetworkBehaviour
{
    void Start()
    {
        if (isLocalPlayer) CameraFollowTarget.SetTargetObject(transform);
    }
}