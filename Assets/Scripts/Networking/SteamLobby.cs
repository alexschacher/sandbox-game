using UnityEngine;
using Mirror;
using Steamworks;

public class SteamLobby : MonoBehaviour
{
    [SerializeField] private GameMenu gameMenu;
    private NetworkManager netManager;
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEnter;
    protected Callback<LobbyKicked_t> lobbyKicked;

    public static CSteamID LobbyID { get; private set; }

    private void Awake()
    {
        netManager = GetComponent<NetworkManager>();
    }

    private void Start()
    {
        if (SteamManager.Initialized)
        {
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
            lobbyKicked = Callback<LobbyKicked_t>.Create(OnLobbyKicked);
        }
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, netManager.maxConnections);
        HUD.LogMessage("SteamLobby: HostLobby called");
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            gameMenu.DisplayMainMenu();
            return;
        }

        LobbyID = new CSteamID(callback.m_ulSteamIDLobby);

        netManager.StartHost();
        SteamMatchmaking.SetLobbyData(LobbyID, "HostAddress", SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        HUD.LogMessage("SteamLobby: GameLobbyJoinRequested");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        HUD.LogMessage("SteamLobby: OnLobbyEnter");

        if (NetworkServer.active) return;

        HUD.LogMessage("SteamLobby: Attempting to start Client...");

        gameMenu.HideMainMenu();
        netManager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress");
        netManager.StartClient();
    }

    private void OnLobbyKicked(LobbyKicked_t callback)
    {
        gameMenu.DisplayMainMenu();
    }
}