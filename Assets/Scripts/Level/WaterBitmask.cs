using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBitmask : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Vector3Int pos;
    [SerializeField] private Mesh waterfallMesh;
    private bool isWaterfall;
    public bool CheckIfWaterfall() => isWaterfall;

    private void Awake()
    {
        pos = new Vector3Int((int)transform.position.x, (int)(transform.position.y * 2), (int)transform.position.z);
        meshFilter = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        if (pos.y > 2)
        {
            if (CheckIfSurrounded(pos.x, pos.y, pos.z) == false)
            {
                BecomeWaterfall();
            }
        }
    }
    private void OnDestroy()
    {
        if (isWaterfall)
        {
            GameObject cliffObject = LevelHandler.GetObjectAtPosition(pos.x, pos.y - 1, pos.z);
            if (cliffObject != null)
            {
                CliffBitmask cliffBitmask = cliffObject.GetComponent<CliffBitmask>();
                if (cliffBitmask != null)
                {
                    cliffBitmask.ChangeCornersForNoWaterfall();
                }
            }
        }
    }

    private void BecomeWaterfall()
    {
        isWaterfall = true;
        meshFilter.mesh = waterfallMesh;

        LevelHandler.ModifyVoxel(vID.Empty, pos.x, pos.y - 2, pos.z);
        LevelHandler.ModifyVoxel(vID.Water, pos.x, pos.y - 3, pos.z);

        GameObject cliffObject = LevelHandler.GetObjectAtPosition(pos.x, pos.y - 1, pos.z);
        if (cliffObject != null)
        {
            CliffBitmask cliffBitmask = cliffObject.GetComponent<CliffBitmask>();
            if (cliffBitmask != null)
            {
                cliffBitmask.ChangeCornersForWaterfall();
            }
        }

        if (CheckIfSurrounded(pos.x, pos.y, pos.z + 1) == true)         transform.rotation = Quaternion.Euler(0, 180, 0);
        else if(CheckIfSurrounded(pos.x, pos.y, pos.z - 1) == true)     transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (CheckIfSurrounded(pos.x + 1, pos.y, pos.z) == true)    transform.rotation = Quaternion.Euler(0, -90, 0);
        else if (CheckIfSurrounded(pos.x - 1, pos.y, pos.z) == true)    transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    private bool CheckIfSurrounded(int x, int y, int z)
    {
        bool north = false;
        bool south = false;
        bool east = false;
        bool west = false;

        if (LevelHandler.GetVoxelIdAtPosition(x, y, z + 1) == vID.Water ||
            LevelHandler.GetVoxelIdAtPosition(x, y + 1, z + 1) == vID.Ground)
        {
            north = true;
        }
        if (LevelHandler.GetVoxelIdAtPosition(x, y, z - 1) == vID.Water ||
            LevelHandler.GetVoxelIdAtPosition(x, y + 1, z - 1) == vID.Ground)
        {
            south = true;
        }
        if (LevelHandler.GetVoxelIdAtPosition(x + 1, y, z) == vID.Water ||
            LevelHandler.GetVoxelIdAtPosition(x + 1, y + 1, z) == vID.Ground)
        {
            east = true;
        }
        if (LevelHandler.GetVoxelIdAtPosition(x - 1, y, z) == vID.Water ||
            LevelHandler.GetVoxelIdAtPosition(x - 1, y + 1, z) == vID.Ground)
        {
            west = true;
        }

        if (north && south && east && west)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}