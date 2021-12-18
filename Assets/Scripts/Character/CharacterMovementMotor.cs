using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterMovementMotor : NetworkBehaviour
{
    private CharCore core;
    private LayerMask solidLayerMask;
    private float colliderRadius = 0.39f;

    [SerializeField] private Vector3 moveDir;
    [SerializeField] private float moveSpeed;
    private float distanceToMove;

    private void Awake()
    {
        core = GetComponent<CharCore>();
        solidLayerMask = LayerMask.GetMask("Solid");
    }
    private void Update()
    {
        if (hasAuthority)
        {
            if (core.GetFishingState() == Fishing.State.NotFishing) // This is our "can we move?" check, also used in character animator
            {
                AttemptMove();

                if (core.GetIntentionDir() != Vector2.zero)
                {
                    core.SetVisualDir(core.GetIntentionDir());
                    core.SetInteractDir(core.GetIntentionDir());
                }
            }
            else
            {
                core.SetWalkDir(Vector2.zero);
            }
        }
    }
    private void AttemptMove()
    {
        moveDir = VectorMath.V2toV3(core.GetIntentionDir());
        distanceToMove = moveSpeed * Time.deltaTime;

        AdjustDirAroundWalls(true);
        AdjustDirAroundCliffs();
        AdjustDirAroundWalls(false);

        if (moveDir != Vector3.zero)
        {
            moveDir.y = 0f;
            transform.Translate(moveDir * distanceToMove);
            core.SetWalkDir(VectorMath.V3toV2Norm(moveDir));
        }
        else
        {
            core.SetWalkDir(Vector2.zero);
        }
        ClampHeightToGround();
    }
    private void AdjustDirAroundWalls(bool allowRedirect)
    {
        RaycastHit hit;
        Vector3 origin = transform.position;

        if (Physics.SphereCast(origin, colliderRadius, moveDir, out hit, distanceToMove, solidLayerMask))
        {
            bool isGround = (Vector3.Dot(hit.normal, Vector3.up) > 0.5f);
            if (isGround) return;

            if (allowRedirect == false)
            {
                moveDir = Vector3.zero;
                return;
            }

            moveDir = Vector3.ProjectOnPlane(moveDir, hit.normal);

            if (Physics.SphereCast(origin, colliderRadius, moveDir, out hit, distanceToMove, solidLayerMask))
            {
                isGround = (Vector3.Dot(hit.normal, Vector3.up) > 0.5f);
                if (isGround) return;

                moveDir = Vector3.zero;
            }
        }
    }
    private void AdjustDirAroundCliffs()
    {
        float rayDownLength = 0.7f + distanceToMove;
        float rayBackLength = colliderRadius + distanceToMove;
        float angle = 60f;
        Vector3 dir = moveDir;

        Vector3 origin = transform.position + (moveDir * (distanceToMove + colliderRadius));
        if (CheckForGround(origin, rayDownLength) == false)
        {
            moveDir = Vector3.ProjectOnPlane(moveDir, FindCliffNormal(origin, rayDownLength, rayBackLength));
        }
        else
        {
            origin = transform.position + VectorMath.RotateAroundY(dir, angle) * (distanceToMove + colliderRadius);
            if (CheckForGround(origin, rayDownLength) == false)
            {
                moveDir = Vector3.ProjectOnPlane(moveDir, FindCliffNormal(origin, rayDownLength, rayBackLength));
            }
            else
            {
                origin = transform.position + VectorMath.RotateAroundY(dir, -angle) * (distanceToMove + colliderRadius);
                if (!CheckForGround(origin, rayDownLength))
                {
                    moveDir = Vector3.ProjectOnPlane(moveDir, FindCliffNormal(origin, rayDownLength, rayBackLength));
                }
            }
        }

        // Ensure we are now walking onto flat ground, otherwise stop
        origin = transform.position + (moveDir * (distanceToMove + colliderRadius));
        if (CheckForGround(origin, rayDownLength) == false)
        {
            moveDir = Vector3.zero;
        }
    }
    private bool CheckForGround(Vector3 origin, float distance)
    {
        RaycastHit hit;

        if (Physics.Raycast(origin, Vector3.down, out hit, distance, solidLayerMask))
        {
            Debug.DrawRay(origin, Vector3.down * distance, Color.blue);
            return true;
        }
        else
        {
            Debug.DrawRay(origin, Vector3.down * distance, Color.red);
            return false;
        }
    }
    private Vector3 FindCliffNormal(Vector3 origin, float rayDownLength, float rayDistance)
    {
        Vector3 dir = (transform.position - origin).normalized;
        RaycastHit hit;

        if (Physics.Raycast(origin + (Vector3.down * rayDownLength), dir, out hit, rayDistance, solidLayerMask))
        {  
            Debug.DrawRay(origin + (Vector3.down * rayDownLength), dir * rayDistance, Color.cyan);
            return hit.normal;
        }
        else
        {
            Debug.DrawRay(origin + (Vector3.down * rayDownLength), dir * rayDistance, Color.magenta);
            return Vector3.zero;
        }
    }
    private void ClampHeightToGround()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        float rayLength = 1.49f;

        if (Physics.Raycast(origin, Vector3.down, out hit, rayLength, solidLayerMask))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.5f, transform.position.z);
        }
    }
}