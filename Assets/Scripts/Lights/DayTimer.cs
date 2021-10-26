using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DayTimer : NetworkBehaviour
{
    public enum PartOfDay { Morning, Day, Evening, Night }
    public enum Season { Spring, Summer, Fall, Winter }

    [SerializeField] private Material material;

    [Header("Time")]
    [SyncVar] [SerializeField] private float currentHour = 12f;
    [SerializeField] private float timeSpeed = 1f;
    private float timeSpeedDefaultNormalizer = 0.1f;
    [SerializeField] private PartOfDay currentPartOfDay = PartOfDay.Day;
    [SerializeField] private float currentLightValue = 1f;
    [SerializeField] private Color currentColor = Color.white;

    [Header("Tint")]
    [SerializeField] private Color morningTint = Color.yellow;
    [SerializeField] private Color dayTint = Color.white;
    [SerializeField] private Color eveningTint = Color.red;
    [SerializeField] private Color nightTint = Color.blue;

    [Header("Daylight")]
    [SerializeField] private float morningLightValue = 0.9f;
    [SerializeField] private float dayLightValue = 1f;
    [SerializeField] private float eveningLightValue = 0.6f;
    [SerializeField] private float nightLightValue = 0f;

    [Header("TransitionTime")]
    [SerializeField] private float morningHour = 5f;
    [SerializeField] private float dayHour = 8f;
    [SerializeField] private float eveningHour = 17f;
    [SerializeField] private float nightHour = 22f;

    [Header("Transition")]
    [SerializeField] private float transitionStartLightValue = 1f;
    [SerializeField] private float transitionTargetLightValue = 1f;
    [SerializeField] private Color transitionStartColor = Color.white;
    [SerializeField] private Color transitionTargetColor = Color.white;
    [SerializeField] private float transitionTimer = 1f;
    [SerializeField] private float transitionLength;

    
    private void Start()
    {
        SetShaderValues();
    }
    private void Update()
    {
        if (isServer)
        {
            UpdateHour();
        }
        CheckForStartTransition();
        LerpTransition();
    }
    private void UpdateHour()
    {
        currentHour += Time.deltaTime * timeSpeed * timeSpeedDefaultNormalizer;
        if (currentHour >= 24f) currentHour = currentHour % 24f;
    }
    private void CheckForStartTransition()
    {
        switch(currentPartOfDay)
        {
            case (PartOfDay.Morning) :
                if (currentHour > dayHour) StartTransition(PartOfDay.Day, dayTint, dayLightValue, 10f);
                break;
            case (PartOfDay.Day) :
                if (currentHour > eveningHour) StartTransition(PartOfDay.Evening, eveningTint, eveningLightValue, 10f);
                break;
            case (PartOfDay.Evening) :
                if (currentHour > nightHour) StartTransition(PartOfDay.Night, nightTint, nightLightValue, 10f);
                break;
            case (PartOfDay.Night) :
                if (currentHour > morningHour && currentHour < nightHour) StartTransition(PartOfDay.Morning, morningTint, morningLightValue, 10f);
                break;
            default: break;
        }
    }
    private void StartTransition(PartOfDay newPartOfDay, Color newColor, float newLightValue, float length)
    {
        currentPartOfDay = newPartOfDay;
        transitionStartLightValue = currentLightValue;
        transitionStartColor = currentColor;
        transitionTargetLightValue = newLightValue;
        transitionTargetColor = newColor;
        transitionLength = length;
        transitionTimer = 0f;
    }
    private void LerpTransition()
    {
        if (transitionTimer > transitionLength) return;
        transitionTimer += Time.deltaTime;

        float lerpValue = transitionTimer / transitionLength;
        currentLightValue = Mathf.Lerp(transitionStartLightValue, transitionTargetLightValue, lerpValue);
        currentColor = new Color(
            Mathf.Lerp(transitionStartColor.r, transitionTargetColor.r, lerpValue),
            Mathf.Lerp(transitionStartColor.g, transitionTargetColor.g, lerpValue),
            Mathf.Lerp(transitionStartColor.b, transitionTargetColor.b, lerpValue));

        SetShaderValues();
    }
    private void SetShaderValues()
    {
        material.SetFloat("_Daylight", currentLightValue);
        material.SetColor("_AtmosphereTint", currentColor);
    }
}