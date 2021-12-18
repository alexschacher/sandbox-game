using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterItemHolder : NetworkBehaviour
{
    private CharCore core;
    [SerializeField] private GameObject heldItemObject;
    private Billboard heldItemBillboard;
    LayerMask itemLayerMask;

    private void Awake()
    {
        itemLayerMask = LayerMask.GetMask("Item");
        heldItemBillboard = heldItemObject.GetComponent<Billboard>();

        core = GetComponent<CharCore>();
        if (core != null)
        {
            core.onHandleItemEvent += HandleItemEvent;
            core.onSetHeldItemEvent += SetHeldItemEvent;
        }
    }

    private void HandleItemEvent()
    {
        if (core.GetHeldItem() == eID.Null)
        {
            AttemptPickupItem();
        }
        else
        {
            AttemptDropItem();
        }
    }

    private void AttemptPickupItem()
    {
        GameObject nearestItem = null;
        float nearestDistance = 1000f;
        Collider[] itemColliders = Physics.OverlapSphere(transform.position, 1f, itemLayerMask);

        foreach (var collider in itemColliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestItem = collider.gameObject;
            }
        }
        if (nearestItem != null)
        {
            EntityObject entity = nearestItem.GetComponent<EntityObject>();
            if (entity != null)
            {
                core.SetHeldItem(entity.GetID());
                LevelHandler.CmdDespawnEntity(nearestItem);
            }
        }
    }

    private void AttemptDropItem()
    {
        LevelHandler.CmdSpawnEntity(core.GetHeldItem(), transform.position);
        core.SetHeldItem(eID.Null);
    }

    private void SetHeldItemEvent()
    {
        eID id = core.GetHeldItem();
        if (id == eID.Null)
        {
            heldItemObject.SetActive(false);
        }
        else
        {
            heldItemObject.SetActive(true);
            EntityBlueprint entityBP = EntityBlueprint.GetFromID(id);
            InitValues_Item itemValues = (InitValues_Item)entityBP.values;
            heldItemBillboard.SetOriginFrame(itemValues.originFrame.x, itemValues.originFrame.y);
        }
    }
}