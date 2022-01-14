using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelObject_Liquid : VoxelObject
{
    public override void Init(InitValues values)
    {
        InitValues_Liquid liquidValues = (InitValues_Liquid)values;

        NonBillboardAnim nonBillboardAnim = GetComponent<NonBillboardAnim>();
        if (nonBillboardAnim == null)
        {
            nonBillboardAnim = GetComponentInChildren<NonBillboardAnim>();
        }
        nonBillboardAnim.SetVariables(liquidValues.animSpeed, liquidValues.animFrames, liquidValues.framePixelWidth);
    }
}