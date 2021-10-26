using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBitmask : MonoBehaviour
{
    private BlockBitmaskInfo info;
    private MeshFilter meshFilter;
    private Vector3Int pos;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        pos = new Vector3Int((int)transform.position.x, (int)(transform.position.y * 2), (int)transform.position.z);
    }

    private void Start()
    {
        UpdateModel();
        UpdateNeighbors();
    }

    public void SetInfo(BlockBitmaskInfo blockBitmaskInfo) => info = blockBitmaskInfo;

    public void UpdateModel()
    {
        Level level = App.GetLevel();

        int bitMask = 0;
        if (level.GetID(pos.x, pos.y, pos.z - 1) == ID.Ground) bitMask += 1;
        if (level.GetID(pos.x - 1, pos.y, pos.z) == ID.Ground) bitMask += 2;
        if (level.GetID(pos.x + 1, pos.y, pos.z) == ID.Ground) bitMask += 4;
        if (level.GetID(pos.x, pos.y, pos.z + 1) == ID.Ground) bitMask += 8;

        switch(bitMask)
        {
            case  0: meshFilter.mesh = info.isleMesh;       transform.rotation = Quaternion.Euler(0, 0, 0); break;
            case  1: meshFilter.mesh = info.endMesh;        transform.rotation = Quaternion.Euler(0, info.rotateEnd + -90, 0); break;
            case  2: meshFilter.mesh = info.endMesh;        transform.rotation = Quaternion.Euler(0, info.rotateEnd, 0); break;
            case  3: meshFilter.mesh = info.cornerMesh;     transform.rotation = Quaternion.Euler(0, info.rotateCorner, 0); break;
            case  4: meshFilter.mesh = info.endMesh;        transform.rotation = Quaternion.Euler(0, info.rotateEnd + 180, 0); break;
            case  5: meshFilter.mesh = info.cornerMesh;     transform.rotation = Quaternion.Euler(0, info.rotateCorner - 90, 0); break;
            case  6: meshFilter.mesh = info.laneMesh;       transform.rotation = Quaternion.Euler(0, info.rotateLane + 90, 0); break;
            case  7: meshFilter.mesh = info.edgeMesh;       transform.rotation = Quaternion.Euler(0, info.rotateEdge - 90, 0); break;
            case  8: meshFilter.mesh = info.endMesh;        transform.rotation = Quaternion.Euler(0, info.rotateEnd + 90, 0); break;
            case  9: meshFilter.mesh = info.laneMesh;       transform.rotation = Quaternion.Euler(0, info.rotateLane, 0); break;
            case 10: meshFilter.mesh = info.cornerMesh;     transform.rotation = Quaternion.Euler(0, info.rotateCorner + 90, 0); break;
            case 11: meshFilter.mesh = info.edgeMesh;       transform.rotation = Quaternion.Euler(0, info.rotateEdge, 0); break;
            case 12: meshFilter.mesh = info.cornerMesh;     transform.rotation = Quaternion.Euler(0, info.rotateCorner + 180, 0); break;
            case 13: meshFilter.mesh = info.edgeMesh;       transform.rotation = Quaternion.Euler(0, info.rotateEdge + 180, 0); break;
            case 14: meshFilter.mesh = info.edgeMesh;       transform.rotation = Quaternion.Euler(0, info.rotateEdge + 90, 0); break;
            case 15: meshFilter.mesh = info.midMesh;        transform.rotation = Quaternion.Euler(0, 0, 0); break;
            default: return;
        }
    }

    private void UpdateNeighbors()
    {
        Level level = App.GetLevel();

        UpdateNeighbor(level, pos.x + 1, pos.y, pos.z);
        UpdateNeighbor(level, pos.x - 1, pos.y, pos.z);
        UpdateNeighbor(level, pos.x, pos.y, pos.z + 1);
        UpdateNeighbor(level, pos.x, pos.y, pos.z - 1);
    }

    private void UpdateNeighbor(Level level, int x, int y, int z)
    {
        GameObject neighbor = level.GetStaticObject(x, y, z);
        if (neighbor != null)
        {
            BlockBitmask blockBitmask = neighbor.GetComponent<BlockBitmask>();
            if (blockBitmask != null)
            {
                blockBitmask.UpdateModel();
            }
        }
    }

    private void OnDestroy() => UpdateNeighbors();
}
