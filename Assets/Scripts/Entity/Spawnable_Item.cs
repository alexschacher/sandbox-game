using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable_Item : Spawnable
{
    private Billboard billboard;

    private void Awake()
    {
        billboard = GetComponent<Billboard>();
    }

    public override void Init(InitValues values)
    {
        InitValues_Item itemValues = (InitValues_Item)values;

        billboard.SetOriginFrame(itemValues.originFrame.x, itemValues.originFrame.y);
    }
}