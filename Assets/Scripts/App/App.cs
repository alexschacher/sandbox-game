using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    public static App singleton;

    public static readonly string gameSaveName = "worldsavetest_newformat6";
    public static readonly string gameSaveName2 = "worldsavetest_new1";
    [SerializeField] private Level_Old level;
    public static Level_Old GetLevel() => singleton.level;

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