using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharCore : NetworkBehaviour
{
    [SerializeField] private Vector2 intentionDir;
    public Vector2 GetIntentionDir() => intentionDir;
    public void SetIntentionDir(Vector2 dir) => intentionDir = dir;

    [SerializeField] private Vector2 interactDir;
    public Vector2 GetInteractDir() => interactDir;
    public void SetInteractDir(Vector2 dir) => interactDir = dir;

    [SerializeField] [SyncVar] private Vector2 visualDir;
    public Vector2 GetVisualDir() => visualDir;
    public void SetVisualDir(Vector2 dir)
    {
        visualDir = dir;
        CmdSetVisualDir(dir);
    }
    [Command] private void CmdSetVisualDir(Vector2 dir) => visualDir = dir;

    [SerializeField] [SyncVar] private Vector2 walkDir;
    public Vector2 GetWalkDir() => walkDir;
    public void SetWalkDir(Vector2 dir)
    {
        walkDir = dir;
        CmdSetWalkDir(dir);
    }
    [Command] private void CmdSetWalkDir(Vector2 dir) => walkDir = dir;

    [SerializeField] [SyncVar] private Fishing.State fishingState;
    public Fishing.State GetFishingState() => fishingState;
    public void SetFishingState(Fishing.State state)
    {
        fishingState = state;
        CmdSetFishingState(state);
    }
    [Command] private void CmdSetFishingState(Fishing.State state) => fishingState = state;

    [SerializeField] [SyncVar(hook = nameof(HookSetHeldItem))]
    private eID heldItem;
    public eID GetHeldItem() => heldItem;
    public void SetHeldItem(eID id)
    {
        heldItem = id;
        CmdSetHeldItem(id);
        onSetHeldItemEvent?.Invoke();
    }
    [Command] private void CmdSetHeldItem(eID id) => heldItem = id;
    private void HookSetHeldItem(eID oldID, eID newID) => onSetHeldItemEvent?.Invoke();

    public event Action onSetHeldItemEvent;
    public event Action onHandleItemEvent;
    public event Action onInteractEvent;

    public void TriggerOnHandleItemEvent() => onHandleItemEvent?.Invoke();
    public void TriggerOnInteractEvent() => onInteractEvent?.Invoke();
} 