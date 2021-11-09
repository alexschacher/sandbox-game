using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelObject_Block : VoxelObject
{
    private BlockBitmask blockBitmask;

    private void Awake()
    {
        blockBitmask = GetComponent<BlockBitmask>();
    }

    public override void Init(InitValues values)
    {
        InitValues_Block blockValues = (InitValues_Block)values;

        blockBitmask.SetInfo(blockValues.blockBitmaskInfo);
    }
}