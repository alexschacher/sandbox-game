using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    public static App instance;

    public static readonly string gameSaveName = "worldsavetest6";
    [SerializeField] private Level level;
    public static Level GetLevel() => instance.level;

    private bool inputLocked = false;
    public static bool IsInputLocked() => instance.inputLocked;
    public static void SetInputLocked(bool value) => instance.inputLocked = value;

    private bool textInputAvailable = false;
    public static bool IsTextInputAvailable() => instance.textInputAvailable;
    public static void SetTextInputAvailable(bool value) => instance.textInputAvailable = value;

    private void Awake()
    {
        if (instance != null) return;
        instance = this;

        Entity.InitDictionary();
    }

    private void OnESC()
    {
        Application.Quit();
    }
}