using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    public static App singleton;

    [SerializeField] private string gameSaveName = "gamesave1";
    public static string GetGameSaveName() => singleton.gameSaveName;

    private bool inputLocked = false;
    public static bool IsInputLocked() => singleton.inputLocked;
    public static void SetInputLocked(bool value) => singleton.inputLocked = value;

    private bool textInputAvailable = false;
    public static bool IsTextInputAvailable() => singleton.textInputAvailable;
    public static void SetTextInputAvailable(bool value) => singleton.textInputAvailable = value;

    private void Awake()
    {
        if (singleton != null) return;
        singleton = this;

        Entity.InitDictionary();
    }

    private void OnESC()
    {
        Application.Quit();
    }
}