using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetManagerAssist : NetworkBehaviour
{
    [ClientRpc] public void RpcUpdateConnectedPlayersText(List<string> displayNames)
    {
        HUD.SetConnectedPlayers(displayNames);
    }

    [ClientRpc] public void RpcSendMessageToAllClients(string message)
    {
        HUD.LogMessage(message);
    }
}