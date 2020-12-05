using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterBrainPlayer : MonoBehaviour
{
    private CharacterIntention intention;

    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction moveInputAction;

    private void Awake()
    {
        intention = GetComponent<CharacterIntention>();
        moveInputAction = inputActionAsset.FindAction("Move");
    }
    private void Update()
    {
        GetMoveInput();
    }
    private void GetMoveInput()
    {
        Vector2 moveInput = moveInputAction.ReadValue<Vector2>();
        Vector2 moveDir = VectorMath.ConvertInputVectorUsingCamera(moveInput, Camera.main.transform);
        intention.SetMoveDir(moveDir);
    }
}