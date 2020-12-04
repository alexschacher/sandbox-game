using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class Command
{
    public string command;
    public UnityEvent response;
}

public class ConsoleCommand : MonoBehaviour
{
    [HideInInspector] public ConsoleCommand INSTANCE;

    [Header("UI Elements")]
    [SerializeField] private InputField inputField;
    [SerializeField] private List<Text> textList;

    [Header("Input")]
    [SerializeField] private KeyCode activateKey = KeyCode.Tab;
    [SerializeField] private KeyCode executeKey = KeyCode.Return;
    [SerializeField] private KeyCode cancelKey = KeyCode.Escape;
    private bool isTakingInput = false;

    [Header("Debug Text")]
    [SerializeField] private bool displayDebugMessages = true;
    [SerializeField] private float messageDisplayTime = 4f;
    private float messageDisplayTimer;

    [Header("Command Definitions")]
    [SerializeField] private List<Command> commands;

    private void InterpretCommand(string textInput)
    {
        SetDebugMessage(textInput);

        string[] strArray = textInput.Split(" "[0]);
        string command = strArray[0];
        string arg = "";
        if (strArray.Length > 1)
        {
            arg = strArray[1];
        }

        foreach (Command c in commands)
        {
            if (command == c.command)
            {
                c.response.Invoke();
                return;
            }
        }

        string msg = "Available Commands:";
        foreach (Command c in commands)
        {
            msg = msg + " " + c.command;
        }
        SetDebugMessage(msg);
    }

    private void Start()
    {
        INSTANCE = this;
    }

    private void Update()
    {
        UpdateMessageDisplayTimer();
        GetInput();
    }

    private void UpdateMessageDisplayTimer()
    {
        if (messageDisplayTimer > 0)
        {
            messageDisplayTimer -= Time.deltaTime;
        }
        else if (messageDisplayTimer > -100f && textList[0] != null)
        {
            textList[0].enabled = false;
            messageDisplayTimer = -101f;
        }
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(cancelKey) && isTakingInput)
        {
            DeactivateInputField();
        }
        else if (Input.GetKeyDown(activateKey) && !isTakingInput)
        {
            ActivateInputField();
        }
        else if (Input.GetKeyDown(executeKey) && isTakingInput)
        {
            if (inputField.text != "")
            {
                InterpretCommand(inputField.text);
            }
            DeactivateInputField();
        }
    }

    private void ActivateInputField()
    {
        SetDebugMessage("");
        isTakingInput = true;
        inputField.ActivateInputField();
        inputField.Select();
    }

    private void DeactivateInputField()
    {
        isTakingInput = false;
        inputField.text = "";
        inputField.DeactivateInputField();
    }

    public void SetDebugMessage(string message)
    {
        SetDebugMessage(message, 0);
    }

    public void SetDebugMessage(string message, int index)
    {
        if (!displayDebugMessages) return;
        if (textList == null) return;
        if (index >= textList.Count) return;

        textList[index].text = message;
        if (index == 0)
        {
            textList[0].enabled = true;
            messageDisplayTimer = messageDisplayTime;
        }
    }
}