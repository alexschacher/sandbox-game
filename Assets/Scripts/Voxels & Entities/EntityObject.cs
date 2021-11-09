using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityObject : NetworkBehaviour
{
    [SyncVar(hook = "SyncEID")] protected eID id;

    public eID GetID() => id;

    [Server] public virtual void ServerInitEntity(eID id)
    {
        this.id = id;
    }

    [Client] protected virtual void ClientInitEntity() { }

    protected void SyncEID(eID oldEID, eID newEID)
    {
        if (isServer) return;
        ClientInitEntity();
    }
}