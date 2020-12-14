using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private InputField userNameInput;
    [SerializeField] private GameObject steamNetManagerObject;
    [SerializeField] private GameObject ipNetManagerObject;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject gameUI;
    private SteamLobby steamLobby;
    private NetworkManager steamNetManager;
    private NetworkManager ipNetManager;
    private NetworkManager activeNetManager;

    public void Awake()
    {
        DisplayMainMenu();
        HideGameUI();

        steamLobby = steamNetManagerObject.GetComponent<SteamLobby>();
        steamNetManager = steamNetManagerObject.GetComponent<NetworkManager>();
        ipNetManager = ipNetManagerObject.GetComponent<NetworkManager>();
    }
    public void HostUsingSteam()
    {
        HideMainMenu();
        DisplayGameUI();

        steamNetManagerObject.SetActive(true);
        activeNetManager = ipNetManager;
        steamLobby.HostLobby();
    }
    public void HostUsingIP()
    {
        HideMainMenu();
        DisplayGameUI();

        ipNetManagerObject.SetActive(true);
        activeNetManager = ipNetManager;
        ipNetManager.StartHost();
    }
    public void JoinIP()
    {
        HideMainMenu();
        DisplayGameUI();

        ipNetManagerObject.SetActive(true);
        activeNetManager = ipNetManager;
        ipNetManager.StartClient();
    }
    public void LeaveGame()
    {
        HideGameUI();
        DisplayMainMenu();

        activeNetManager.StopHost();
        activeNetManager.StopServer();
        activeNetManager.StopClient();

        steamNetManagerObject.SetActive(false);
        ipNetManagerObject.SetActive(false);
    }
    public void DisplayMainMenu() => menuUI.SetActive(true);
    public void HideMainMenu() => menuUI.SetActive(false);
    public void DisplayGameUI() => gameUI.SetActive(true);
    public void HideGameUI() => gameUI.SetActive(false);
    public void ExitGame() => Application.Quit();
}