using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ToggleTorch : NetworkBehaviour
{
    [SerializeField] private GameObject torch;

    [SyncVar(hook = "Hook_SetTorchToggle")] private bool torchOn = true;
    [Command] private void CmdSetTorchToggle(bool toggle) => torchOn = toggle;
    private void Hook_SetTorchToggle(bool oldValue, bool newValue) => torch.SetActive(newValue);

    private void OnGrab()
    {
        if (hasAuthority && !App.IsInputLocked())
        {
            torchOn = !torchOn;
            torch.SetActive(torchOn);
            CmdSetTorchToggle(torchOn);
        }
    }
}