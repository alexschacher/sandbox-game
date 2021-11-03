using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject steamNetManagerObject;
    [SerializeField] private GameObject ipNetManagerObject;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject levelEditCursor;

    [SerializeField] private GameObject steamHostButton;
    [SerializeField] private GameObject localHostButton;
    [SerializeField] private GameObject joinLocalButton;

    private SteamLobby steamLobby;
    private NetworkManager steamNetManager;
    private NetworkManager ipNetManager;
    private NetworkManager activeNetManager;
    private bool inEditor = false;

    public void Awake()
    {
        DisplayMainMenu();
        HideGameUI();

        steamLobby = steamNetManagerObject.GetComponent<SteamLobby>();
        steamNetManager = steamNetManagerObject.GetComponent<NetworkManager>();
        ipNetManager = ipNetManagerObject.GetComponent<NetworkManager>();

        #if UNITY_EDITOR
                inEditor = true;
        #endif
        if (inEditor)
        {
            ipNetManagerObject.SetActive(true);
            Destroy(steamNetManagerObject);
            activeNetManager = ipNetManager;
            steamHostButton.SetActive(false);
        }
        else
        {
            steamNetManagerObject.SetActive(true);
            Destroy(ipNetManagerObject);
            activeNetManager = steamNetManager;
            localHostButton.SetActive(false);
            joinLocalButton.SetActive(false);
        }
    }

    public void HostUsingSteam()
    {
        if (inEditor == true) return;

        HideMainMenu();
        DisplayGameUI();
        App.SetTextInputAvailable(true);

        levelEditCursor.SetActive(true);
        HUD.SetSelectedIDVisibility(true);

        steamLobby.HostLobby();
    }

    public void HostUsingIP()
    {
        if (inEditor == false) return;

        HideMainMenu();
        DisplayGameUI();
        App.SetTextInputAvailable(true);

        levelEditCursor.SetActive(true);
        HUD.SetSelectedIDVisibility(true);

        ipNetManager.StartHost();
    }

    public void JoinIP()
    {
        if (inEditor == false) return;

        HideMainMenu();
        DisplayGameUI();
        App.SetTextInputAvailable(true);

        ipNetManager.StartClient();
    }

    public void LeaveGame()
    {
        HideGameUI();
        DisplayMainMenu();
        App.SetTextInputAvailable(false);

        levelEditCursor.SetActive(false);
        HUD.SetSelectedIDVisibility(false);

        activeNetManager.StopHost();
        activeNetManager.StopServer();
        activeNetManager.StopClient();

        HUD.LogMessage("Left game");
    }

    public void DisplayMainMenu() => menuUI.SetActive(true);
    public void HideMainMenu() => menuUI.SetActive(false);
    public void DisplayGameUI() => gameUI.SetActive(true);
    public void HideGameUI() => gameUI.SetActive(false);

    public void ExitGame() => Application.Quit();
}