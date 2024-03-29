﻿using UnityEngine;

[RequireComponent(typeof(CameraController))]
public class CameraPanWithAxis : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private string horizontalInputAxis = "Horizontal";
    [SerializeField] private string verticalInputAxis = "Vertical";
    [SerializeField] private bool invertHorizontalAxis = false;
    [SerializeField] private bool invertVerticalAxis = false;

    [Header("Motion")]
    [SerializeField][Range(0.3f, 10f)] private float speed = 1f;

    private Transform cam;

    private void Start()
    {
        cam = GetComponent<CameraController>().Cam;
    }

    private void Update()
    {
        if (CameraFollowTarget.IsFollowingTarget()) return;

        Pan();
    }

    private void Pan()
    {
        float horizontalInput = Input.GetAxisRaw(horizontalInputAxis);
        float verticalInput = Input.GetAxisRaw(verticalInputAxis);

        if (invertHorizontalAxis)
        {
            horizontalInput = -horizontalInput;
        }
        if (invertVerticalAxis)
        {
            verticalInput = -verticalInput;
        }
        
        Vector2 input = new Vector2(horizontalInput, verticalInput);
        Vector2 moveDir = VectorMath.ConvertInputToWorldDir(input, cam);

        transform.position += new Vector3(moveDir.x, 0f, moveDir.y) * speed * Time.deltaTime * 15;
    }
}