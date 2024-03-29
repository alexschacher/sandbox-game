﻿using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class CameraFollowTarget : MonoBehaviour
{
    [HideInInspector] private static CameraFollowTarget Instance;

    [SerializeField] private Material material;

    [Header("Target")]
    [SerializeField] public bool followTarget = true;
    [SerializeField] private Transform targetObject;
    [SerializeField] private Vector3 targetOffset = Vector3.zero;

    [Header("Motion")]
    [SerializeField] private float followLag = 0.25f;
    private Vector3 followVelocity = Vector3.zero;

    private void Awake()
    {
        Instance = (Instance == null) ? this : Instance;
    }

    private void Update()
    {
        if (targetObject != null && followTarget)
        {
            FollowTarget();
            UpdateMaterial();
        }
    }

    private void FollowTarget()
    {
        transform.position = Vector3.SmoothDamp(
        transform.position,
        targetObject.position + targetOffset,
        ref followVelocity,
        followLag
        );
    }

    private void UpdateMaterial()
    {
        material.SetVector("_CameraTargetPosition", targetObject.transform.position);
    }

    public static void SetTargetObject(Transform target)
    {
        if (Instance == null) return;
        Instance.targetObject = target;
    }

    public static void SetFollowTarget(bool follow)
    {
        if (Instance == null) return;
        Instance.followTarget = follow;
    }

    public static bool IsFollowingTarget()
    {
        if (Instance == null)
        {
            return false;
        }
        if (Instance.followTarget == true && Instance.targetObject != null)
        {
            return true;
        }
        return false;
    }
}