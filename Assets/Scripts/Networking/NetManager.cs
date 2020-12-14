using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetManager : NetworkManager
{
    [Header("Custom Fields")]
    [SerializeField] private GameMenu gameMenu;

    override public void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        gameMenu.DisplayMainMenu();
        gameMenu.HideGameUI();
    }
}