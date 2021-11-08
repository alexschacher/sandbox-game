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

    private float updateLiveChunksTimer = 0;
    private float updateLiveChunksInterval = 1.5f;
    private List<GameObject> playerCharacterObjects = new List<GameObject>();

    [Header("Custom Fields")]
    [SerializeField] private GameMenu gameMenu;
    [SerializeField] private GameObject playerCharacterPrefab;
    [SerializeField] private GameObject netManagerAssistPrefab;
    [SerializeField] private bool isSteamNetManager;

    override public void Awake()
    {
        base.Awake();

        if (instance != null) return;
        instance = this;
    }

    private void Update()
    {
        if (!NetworkServer.active) return;

        updateLiveChunksTimer += Time.deltaTime;
        if (updateLiveChunksTimer > updateLiveChunksInterval)
        {
            updateLiveChunksTimer -= updateLiveChunksInterval;
            LevelHandler.UpdateLiveChunks(playerCharacterObjects);
        }
    }

    override public void OnStartServer()
    {
        base.OnStartServer();

        LevelHandler.InitHostLevel();
        
        GameObject netManagerAssistObj = Instantiate(netManagerAssistPrefab);
        NetworkServer.Spawn(netManagerAssistObj);
        netManagerAssist = netManagerAssistObj.GetComponent<NetManagerAssist>();
    }

    override public void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        GameObject playerObj = Instantiate(playerCharacterPrefab, new Vector3(15f, 0.5f, 15f), Quaternion.identity);
        NetworkServer.Spawn(playerObj, conn);
        playerCharacterObjects.Add(playerObj);
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

        LevelHandler.SendInitLevelInfo(conn);
        LevelHandler.SendAllLiveChunksToClient(conn);
        LevelHandler.UpdateLiveChunks(playerCharacterObjects);

    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        LevelUtil.SaveLevel(LevelHandler.GetLevel(), App.GetGameSaveName());

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

        LevelHandler.DestroyObjectsInAllChunks();
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