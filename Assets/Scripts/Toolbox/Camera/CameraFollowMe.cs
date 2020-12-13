using UnityEngine;
using Mirror;

public class CameraFollowMe : NetworkBehaviour
{
    override public void OnStartAuthority()
    {
        CameraFollowTarget.SetTargetObject(transform);
    }
}