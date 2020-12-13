using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class CharacterBrainPlayer : NetworkBehaviour
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
        if (isLocalPlayer)
        {
            GetMoveInput();
        }
    }
    private void GetMoveInput()
    {
        Vector2 moveInput = moveInputAction.ReadValue<Vector2>();
        Vector2 moveDir = VectorMath.ConvertInputToWorldDir(moveInput, Camera.main.transform);
        intention.SetMoveDir(moveDir);
    }
}