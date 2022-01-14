using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class CharacterBrainPlayer : NetworkBehaviour
{
    private CharCore core;

    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction moveInputAction;

    private void Awake()
    {
        core = GetComponent<CharCore>();
        moveInputAction = inputActionAsset.FindAction("Move");
    }

    private void Update()
    {
        if (hasAuthority)
        {
            if (App.IsInputLocked())
            {
                core.SetIntentionDir(Vector2.zero);
            }
            else
            {
                GetIntentionDirInput();
            }
        }
    }

    private void GetIntentionDirInput()
    {
        Vector2 intentionDirInput = moveInputAction.ReadValue<Vector2>();
        Vector2 intentionDir = VectorMath.ConvertInputToWorldDir(intentionDirInput, Camera.main.transform);
        core.SetIntentionDir(intentionDir);
    }

    private void OnInputHandleItem()
    {
        if (hasAuthority && !App.IsInputLocked())
        {
            core.TriggerOnHandleItemEvent();
        }
    }
    private void OnInputInteract()
    {
        if (hasAuthority && !App.IsInputLocked())
        {
            core.LoseHealth(33f);
            //core.TriggerOnInteractEvent();
        }
    }
}