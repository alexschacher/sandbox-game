using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD instance;
    
    private void Awake()
    {
        instance = this;
        InitiateMessageDisplayTimers();
        ShowOnlyCurrentMessages();
        DeactivateInputField();
    }

    private void Update()
    {
        UpdateMessageDisplayTimers();
    }

    #region MessageBox Input

    [SerializeField] private InputField inputField;
    [SerializeField] private GameObject inputFieldObj;
    private bool isTakingInput = false;
    public delegate void SendMessageHandler(string message);
    public static event SendMessageHandler SendMessageEvent;

    private void OnEnter()
    {
        if (App.IsTextInputAvailable() == false)
        {
            if (isTakingInput)
            {
                DeactivateInputField();
            }
            return;
        }
        if (isTakingInput)
        {
            if (inputField.text != "")
            {
                SendMessageEvent(inputField.text);
            }
            DeactivateInputField();
            return;
        }
        else
        {
            ActivateInputField();
        }
    }

    private void ActivateInputField()
    {
        isTakingInput = true;
        inputFieldObj.SetActive(true);
        inputField.ActivateInputField();
        inputField.Select();
        App.SetInputLocked(true);
        ShowAllMessages();
        connectedPlayersText.gameObject.SetActive(true);
    }

    private void DeactivateInputField()
    {
        isTakingInput = false;
        inputField.text = "";
        inputField.DeactivateInputField();
        inputFieldObj.SetActive(false);
        App.SetInputLocked(false);
        ShowOnlyCurrentMessages();
        connectedPlayersText.gameObject.SetActive(false);
    }
    #endregion

    #region MessageBox

    [SerializeField] private List<GameObject> messageLogObjs;
    private List<string> messagesList = new List<string>();
    private List<float> messageDisplayTimers = new List<float>();
    private float messageDisplayTime = 4f;

    public static void LogMessage(string text) => instance.AddMessage(text);
    public static void LogError(string text) => instance.AddMessage("[ERROR] " + text);
    
    private void AddMessage(string text)
    {
        while (messagesList.Count > messageLogObjs.Count - 1)
        {
            messagesList.RemoveAt(messageLogObjs.Count - 1);
        }
        messagesList.Insert(0, text);

        for (int i = messageDisplayTimers.Count - 1; i > 0; i--)
        {
            messageDisplayTimers[i] = messageDisplayTimers[i - 1];
        }
        messageDisplayTimers[0] = 4f;

        for (int i = 0; i < messagesList.Count; i++)
        {
            messageLogObjs[i].GetComponentInChildren<Text>().text = messagesList[i];
        }
        ShowOnlyCurrentMessages();
    }

    private void InitiateMessageDisplayTimers()
    {
        foreach (GameObject obj in messageLogObjs)
        {
            messageDisplayTimers.Add(-101f);
        }
    }

    private void UpdateMessageDisplayTimers()
    {
        for (int i = 0; i < messageDisplayTimers.Count; i++)
        {
            if (messageDisplayTimers[i] > -100)
            {
                messageDisplayTimers[i] -= Time.deltaTime;

                if (messageDisplayTimers[i] < 0)
                {
                    messageDisplayTimers[i] = -101;

                    if (isTakingInput == false)
                    {
                        messageLogObjs[i].SetActive(false);
                    }
                }
            }
        }
    }

    private void ShowOnlyCurrentMessages()
    {
        for (int i = 0; i < messageDisplayTimers.Count; i++)
        {
            if (messageDisplayTimers[i] < -100)
            {
                messageLogObjs[i].SetActive(false);
            }
            else
            {
                messageLogObjs[i].SetActive(true);
            }
        }
    }

    private void ShowAllMessages()
    {
        for (int i = 0; i < messagesList.Count; i++)
        {
            messageLogObjs[i].SetActive(true);
        }
    }
    #endregion

    #region Selected ID

    [SerializeField] private Text selectedIdText;

    public static void SetSelectedID(string message)
    {
        instance.selectedIdText.text = "Selected: " + message;
    }
    public static void SetSelectedIDVisibility(bool value)
    {
        instance.selectedIdText.gameObject.SetActive(value);
    }

    #endregion

    #region ConnectedPlayers

    [SerializeField] private Text connectedPlayersText;

    public static void SetConnectedPlayers(List<string> displayNames)
    {
        string text = "Players:";
        foreach (string displayName in displayNames)
        {
            text = text + System.Environment.NewLine + " " + displayName;
        }
        instance.connectedPlayersText.text = text;
    }
    #endregion
}