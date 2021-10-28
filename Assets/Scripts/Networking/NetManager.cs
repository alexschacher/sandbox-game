using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public class NetManager : NetworkManager
{
    private static NetManager instance;
    private List<NetPlayer> listOfConnectedPlayers = new List<NetPlayer>();
    private NetManagerAssist netManagerAssist;

    [Header("Custom Fields")]
    [SerializeField] private GameMenu gameMenu;
    [SerializeField] private GameObject playerCharacterPrefab;
    [SerializeField] private GameObject netManagerAssistPrefab;
    [SerializeField] private bool isSteamNetManager;

    override public void Awake()
    {
        base.Awake();
        instance = this;
    }

    override public void OnStartServer()
    {
        base.OnStartServer();

        if (LevelSaveLoad.LoadLevel(App.gameSaveName) == true) { }
        else
        {
            App.GetLevel().GenerateLevel(); // Delete this
            Level level = LevelGenerator.Generate(8);
            HUD.LogMessage("NetManager: New Level Generated");
        }

        GameObject netManagerAssistObj = Instantiate(netManagerAssistPrefab);
        NetworkServer.Spawn(netManagerAssistObj);
        netManagerAssist = netManagerAssistObj.GetComponent<NetManagerAssist>();
    }

    override public void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // Instead of this, send chunks manually
        App.GetLevel().SendLevelDataToClient(conn);

        GameObject playerObj = Instantiate(playerCharacterPrefab, new Vector3(3f, 0.5f, 3f), Quaternion.identity);
        NetworkServer.Spawn(playerObj, conn);
        uint controlledEntityNetId = playerObj.GetComponent<NetworkIdentity>().netId;

        NetPlayer netPlayer = conn.identity.GetComponent<NetPlayer>();
        listOfConnectedPlayers.Add(netPlayer);
        
        if (isSteamNetManager)
        {
            CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.LobbyID, numPlayers - 1);
            netPlayer.Init(SteamFriends.GetFriendPersonaName(steamId), controlledEntityNetId);
        }
        else
        {
            netPlayer.Init("Player " + numPlayers, controlledEntityNetId);
        }

        UpdateConnectedPlayersText();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        // Instead of this, save new level
        LevelSaveLoad.Save(App.gameSaveName);

        listOfConnectedPlayers.Clear();
        UpdateConnectedPlayersText();
    }

    public override void OnStopClient()
    {
        listOfConnectedPlayers.Clear();
        UpdateConnectedPlayersText();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        listOfConnectedPlayers.Remove(conn.identity.GetComponent<NetPlayer>());
        UpdateConnectedPlayersText();
        base.OnServerDisconnect(conn);
    }

    override public void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        gameMenu.DisplayMainMenu();
        gameMenu.HideGameUI();
        App.GetLevel().DestroyAllStaticEntities();
    }

    [Server] private void UpdateConnectedPlayersText()
    {
        List<string> displayNames = new List<string>();
        foreach(NetPlayer netplayer in listOfConnectedPlayers)
        {
            displayNames.Add(netplayer.GetDisplayName());
        }
        netManagerAssist.RpcUpdateConnectedPlayersText(displayNames);
    }

    [Server] public static void SendMessageToAllClients(string message)
    {
        instance.netManagerAssist.RpcSendMessageToAllClients(message);
    }
}