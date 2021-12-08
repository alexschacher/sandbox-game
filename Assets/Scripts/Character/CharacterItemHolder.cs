﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterItemHolder : NetworkBehaviour
{
    [SerializeField] private bool isHoldingItem;
    [SerializeField] private eID heldItem;
    LayerMask itemLayerMask;

    private void Awake()
    {
        itemLayerMask = LayerMask.GetMask("Item");
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
        if (isHoldingItem)
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
                heldItem = entity.GetID();
                isHoldingItem = true;
                LevelHandler.CmdDespawnEntity(nearestItem);
            }
        }
    }

    private void AttemptDropItem()
    {
        LevelHandler.CmdSpawnEntity(heldItem, transform.position);
        isHoldingItem = false;
        heldItem = eID.Null;
    }
}