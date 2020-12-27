using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveGame : MonoBehaviour
{
    public static ActiveGame instance;
    public static readonly string gameSaveName = "worldsavetest";
    [SerializeField] private Level level;

    private void Awake()
    {
        if (instance != null) return;
        instance = this;

        Entity.InitDictionary();
    }

    public static Level GetLevel() => instance.level;
}