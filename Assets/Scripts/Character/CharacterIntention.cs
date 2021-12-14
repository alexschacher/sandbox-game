using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIntention : MonoBehaviour
{
    private Vector2 aimDir;
    public Vector2 GetAimDir() => aimDir;
    public void SetAimDir(Vector2 dir) => aimDir = dir;

    private CharacterActionState actionState;
    public CharacterActionState GetActionState() => actionState;
    public void SetActionState(CharacterActionState value) => actionState = value;

    public event Action onHandleItemEvent;
    public event Action onInteractEvent;
    public void TriggerOnHandleItemEvent() => onHandleItemEvent?.Invoke();
    public void TriggerOnInteractEvent() => onInteractEvent?.Invoke();
}

public enum CharacterActionState
{
    Default,
    Fishing
}