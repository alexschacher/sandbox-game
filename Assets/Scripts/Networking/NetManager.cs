using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetManager : NetworkManager
{
    [Header("Custom Fields")]
    [SerializeField] private GameMenu gameMenu;

    public override void OnStartServer()
    {
        base.OnStartServer();

        ActiveGame.GetLevel().GenerateLevel();
    }

    override public void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        ActiveGame.GetLevel().SendLevelDataToClient(conn);
    }

    override public void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        gameMenu.DisplayMainMenu();
        gameMenu.HideGameUI();
        ActiveGame.GetLevel().DestroyAllStaticEntities();
    }
}