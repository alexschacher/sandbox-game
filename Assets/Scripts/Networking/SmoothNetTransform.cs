using UnityEngine;
using Mirror;

public class SmoothNetTransform : NetworkBehaviour
{
    [SerializeField] private float updatesPerSecond;
    private float updateTimer;
    private float lastUpdateTime;

    private Vector3 lerpStartPosition;
    private Vector3 lerpEndPosition;
    private float lerpTime;
    private float lerpTimer;

    private void Awake()
    {
        lastUpdateTime = Time.time;
        lerpStartPosition = transform.position;
        lerpEndPosition = transform.position;
    }
    private void Update()
    {
        if (hasAuthority)
        {
            UpdatePosition();
        }
        else
        {
            LerpPosition();
        }
    }
    private void UpdatePosition()
    {
        updateTimer += Time.deltaTime;

        if (updateTimer >= 1f / updatesPerSecond)
        {
            updateTimer -= 1f / updatesPerSecond;

            CmdSendNewPosition(transform.position, Time.time - lastUpdateTime);
            lastUpdateTime = Time.time;
        }
    }
    [Command] private void CmdSendNewPosition(Vector3 newPosition, float timeSinceLast)
    {
        RpcSendNewPosition(newPosition, timeSinceLast);
    }
    [ClientRpc] private void RpcSendNewPosition(Vector3 newPosition, float timeSinceLast)
    {
        if (hasAuthority) return;

        lerpStartPosition = transform.position;
        lerpEndPosition = newPosition;
        lerpTime = timeSinceLast;
        lerpTimer = 0f;
    }
    private void LerpPosition()
    {
        lerpTimer += Time.deltaTime;
        transform.position = Vector3.Lerp(lerpStartPosition, lerpEndPosition, lerpTimer / lerpTime);
    }
}