using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayer : NetworkBehaviour
{
    [SyncVar] private string displayName = "missingNo";
    public string GetDisplayName() => displayName;
    [SyncVar] private uint controlledEntityNetId = 0;

    public override void OnStartClient()
    {
        base.OnStartClient();
        DontDestroyOnLoad(this);

        if (displayName != "missingNo" && controlledEntityNetId != 0)
        {
            // if you are a joining player, sets players who existed before you, but not you or newer
            // because newly spawned players will not yet have their info set and synced
            SetDisplayNameToEntity(displayName, controlledEntityNetId); 
        }
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        HUD.SendMessageEvent += CmdSendChatMessage;
    }

    [Server] public void Init(string newDisplayName, uint newNetId)
    {
        displayName = newDisplayName;
        controlledEntityNetId = newNetId;
        RpcSetDisplayNameToEntity(newDisplayName, newNetId);
    }

    [ClientRpc] private void RpcSetDisplayNameToEntity(string newDisplayName, uint newNetId)
    {
        // if you are a joining player, sets your own name and anyone newer than you, but no one who existed before you
        // because this happens only when new players are created
        SetDisplayNameToEntity(newDisplayName, newNetId); 
        HUD.LogMessage("Player joined: " + newDisplayName);
    }

    [Client] private void SetDisplayNameToEntity(string newDisplayName, uint newNetId)
    {
        if (!NetworkIdentity.spawned.ContainsKey(newNetId))
        {
            HUD.LogError("Trying to access invalid NetID: " + newNetId + " for user " + newDisplayName);
            return;
        }
        GameObject playerObj = NetworkIdentity.spawned[newNetId].gameObject;
        if (playerObj == null)
        {
            HUD.LogError("PlayerObject not found for user " + newDisplayName);
        }
        UiFollowObject ui = playerObj.GetComponent<UiFollowObject>();
        if (ui != null)
        {
            ui.SetText(newDisplayName);
        }
        else
        {
            HUD.LogError("UiFollowObject not found for user " + newDisplayName);
        }
    }

    [Command] public void CmdSendChatMessage(string message)
    {
        NetManager.SendMessageToAllClients(displayName + ": " + message);
    }
}