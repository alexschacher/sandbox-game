using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBitmask : MonoBehaviour
{
    private Vector3Int pos;

    private void Awake()
    {
        pos = new Vector3Int((int)transform.position.x, (int)(transform.position.y * 2), (int)transform.position.z);
    }

    private void Start()
    {
        UpdateNeighborCliffs();
        CheckForAndRemoveCliff(pos.x, pos.z);
    }
    private void OnDestroy()
    {
        UpdateNeighborCliffs();
        UpdateCliff(pos.x, pos.z);
    }

    private void UpdateNeighborCliffs()
    {
        UpdateCliff(pos.x + 1, pos.z);
        UpdateCliff(pos.x - 1, pos.z);
        UpdateCliff(pos.x, pos.z + 1);
        UpdateCliff(pos.x, pos.z - 1);

        UpdateCliff(pos.x + 1, pos.z + 1);
        UpdateCliff(pos.x + 1, pos.z - 1);
        UpdateCliff(pos.x - 1, pos.z + 1);
        UpdateCliff(pos.x - 1, pos.z - 1);
    }

    private void UpdateCliff(int x, int z)
    {
        if (LevelHandler.GetVoxelIdAtPosition(x, pos.y, z) == vID.Ground) return;

        vID cliffID = LevelHandler.GetVoxelIdAtPosition(x, pos.y - 2, z);
        if (cliffID != vID.Cliff)
        {
            // TODO Should be restricted to only Host doing this..
            LevelHandler.ModifyVoxel(vID.Cliff, x, pos.y - 2, z);
        }
        else
        {
            GameObject cliffObject = LevelHandler.GetObjectAtPosition(x, pos.y - 2, z);

            if (cliffObject != null)
            {
                CliffBitmask cliffBitmask = cliffObject.GetComponent<CliffBitmask>();

                if (cliffBitmask != null)
                {
                    cliffBitmask.UpdateModel();
                }
                else
                {
                    //Debug.Log("GroundBitmask: Cliff bitmask not found when expected");
                }
            }
            else
            {
                //Debug.Log("GroundBitmask: Cliff object not found when expected");
            }
        }
    }

    private void CheckForAndRemoveCliff(int x, int z)
    {
        if (LevelHandler.GetVoxelIdAtPosition(x, pos.y - 2, z) == vID.Cliff)
        {
            // TODO Should be restricted to only Host doing this..
            LevelHandler.ModifyVoxel(vID.Empty, x, pos.y - 2, z);
        }
    }
}