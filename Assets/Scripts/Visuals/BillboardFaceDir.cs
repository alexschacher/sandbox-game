using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Billboard))]
public class BillboardFaceDir : MonoBehaviour
{
    private Billboard billboard;
    private CharCore core;
    private bool flipX = false;
    private bool prevFlipX = false;

    private void Awake()
    {
        billboard = GetComponent<Billboard>();
        core = GetComponent<CharCore>();
    }
    private void Update()
    {
        flipX = VectorMath.DetermineBillboardFlipX(core.GetVisualDir(), flipX, Camera.main.transform);

        if (flipX != prevFlipX)
        {
            billboard.SetFlipX(flipX);
        }
        
        prevFlipX = flipX;
    }
}