using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityObject_Item : EntityObject
{
    private Billboard billboard;

    private void Awake()
    {
        EnsureInit();
    }

    private void EnsureInit()
    {
        billboard = GetComponent<Billboard>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isServer) return;

        EnsureInit();
        SyncEID(id, id);
    }

    public override void ServerInitEntity(eID id)
    {
        base.ServerInitEntity(id);
        InitEntity();
    }

    protected override void ClientInitEntity()
    {
        base.ClientInitEntity();
        InitEntity();
    }

    private void InitEntity()
    {
        EntityBlueprint entityBP = EntityBlueprint.GetFromID(id);
        InitValues_Item itemValues = (InitValues_Item)entityBP.values;
        billboard.SetOriginFrame(itemValues.originFrame.x, itemValues.originFrame.y);
    }
}