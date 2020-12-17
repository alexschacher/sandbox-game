using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppInit : MonoBehaviour
{
    private void Awake()
    {
        Entity.InitDictionary();
    }
}
