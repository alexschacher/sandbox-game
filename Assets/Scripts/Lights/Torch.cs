using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    [SerializeField] private GameObject lightObject;
    private Light lightSource;
    [SerializeField] private float radiusVariance = 0.1f;
    [SerializeField] private float offsetVariance = 0.1f;
    [SerializeField] private float changeRate = 0.2f;
    private float changeTimer;
    private Vector3 startPosition;
    private float startRadius;

    private void Awake()
    {
        startPosition = lightObject.transform.position;
        lightSource = lightObject.GetComponent<Light>();
        startRadius = lightSource.range;
    }

    private void Update()
    {
        changeTimer += Time.deltaTime;
        if (changeTimer > changeRate)
        {
            changeTimer = changeTimer % changeRate;
            lightSource.range = startRadius + Random.Range(-radiusVariance, radiusVariance);
            lightObject.transform.position = startPosition + new Vector3(
                Random.Range(-offsetVariance, offsetVariance),
                Random.Range(-offsetVariance, offsetVariance),
                Random.Range(-offsetVariance, offsetVariance));
        }
    }
}