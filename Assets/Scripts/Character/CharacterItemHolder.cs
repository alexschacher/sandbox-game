using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterItemHolder : NetworkBehaviour
{
    [SerializeField] private eID heldItem;
    public eID GetHeldItem() => heldItem;
    public void SetHeldItem(eID item)
    {
        heldItem = item;
        if (heldItem == eID.Null)
        {
            heldItemObject.SetActive(false);
        }
        else
        {
            heldItemObject.SetActive(true);
            EntityBlueprint entityBP = EntityBlueprint.GetFromID(heldItem);
            InitValues_Item itemValues = (InitValues_Item)entityBP.values;
            heldItemBillboard.SetOriginFrame(itemValues.originFrame.x, itemValues.originFrame.y);
        }
    }

    [SerializeField] private GameObject heldItemObject;
    private Billboard heldItemBillboard;
    LayerMask itemLayerMask;

    private void Awake()
    {
        itemLayerMask = LayerMask.GetMask("Item");
        heldItemBillboard = heldItemObject.GetComponent<Billboard>();
    }

    private void Start()
    {
        CharacterIntention intention = GetComponent<CharacterIntention>();
        if (intention != null)
        {
            intention.onHandleItemEvent += HandleItemEvent;
        }
    }

    private void HandleItemEvent()
    {
        if (heldItem != eID.Null)
        {
            AttemptDropItem();
        }
        else
        {
            AttemptPickupItem();
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
                SetHeldItem(entity.GetID());
                LevelHandler.CmdDespawnEntity(nearestItem);
            }
        }
    }

    private void AttemptDropItem()
    {
        LevelHandler.CmdSpawnEntity(heldItem, transform.position);
        SetHeldItem(eID.Null);
    }
}