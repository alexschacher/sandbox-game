using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Level : NetworkBehaviour
{
    private short[] levelIDs;
    public short[] GetLevelData() => levelIDs;
    private GameObject[,,] staticObjects;

    [SerializeField] private int levelWidth = 32;
    public int GetLevelWidth() => levelWidth;

    [SerializeField] private int levelHeight = 4;
    public int GetLevelHeight() => levelHeight;



    // Generate Level Data
    private void Awake()
    {
        staticObjects = new GameObject[levelWidth, levelHeight, levelWidth];
    }
    
    public void GenerateLevel()
    {
        short[] levelData = new short[levelWidth * levelWidth * levelHeight];

        for (int z = 0; z < levelWidth; z++)
        {
            for (int x = 0; x < levelWidth; x++)
            {
                levelData[Get1D(x, 0, z)] = (short)ID.Ground;

                if (x == 0 || z == 0 || x == levelWidth - 1 || z == levelWidth - 1)
                {
                    levelData[Get1D(x, 0, z)] = (short)ID.Water;
                }
                else if (x == 1 || z == 1 || x == levelWidth - 2 || z == levelWidth - 2)
                {
                    levelData[Get1D(x, 1, z)] = (short)ID.Post;
                }
                else
                {
                    if (Random.Range(0f, 100f) < 2f)
                    {
                        levelData[Get1D(x, 1, z)] = (short)ID.Apple;
                    }
                    else
                    if (Random.Range(0f, 100f) < 5f)
                    {
                        levelData[Get1D(x, 1, z)] = (short)ID.Tree;
                    }
                }
            }
        }
        levelIDs = levelData;
    }


    // Save and Load Level Data

    public void SetLevelData(int width, int height, short[] levelData)
    {
        levelWidth = width;
        levelHeight = height;
        levelIDs = levelData;
    }



    // Send Level Data to Client
    public void SendLevelDataToClient(NetworkConnection conn)
    {
        TargetSendLevelData(conn, levelIDs);
    }

    [TargetRpc]
    private void TargetSendLevelData(NetworkConnection target, short[] levelData)
    {
        levelIDs = levelData;
        DestroyAllStaticEntities();
        SpawnLevel(levelData);
    }



    // Spawn Objects
    private void SpawnLevel(short[] levelData)
    {
        for (int y = 0; y < levelHeight; y++)
        {
            for (int z = 0; z < levelWidth; z++)
            {
                for (int x = 0; x < levelWidth; x++)
                {
                    SpawnObject((ID)levelData[Get1D(x, y, z)], x, y, z);
                }
            }
        }
    }

    private void SpawnObject(ID id, int x, int y, int z)
    {
        if (id == ID.Empty) return;

        Entity entity = Entity.GetFromID(id);
        GameObject spawnedObj = Instantiate(entity.prefab, new Vector3(x, y * 0.5f, z), Quaternion.identity, transform);
        spawnedObj.GetComponent<Spawnable>().Init(entity.values);
        staticObjects[x, y, z] = spawnedObj;
    }



    // Destroy Objects
    public void DestroyAllStaticEntities()
    {
        foreach (GameObject obj in staticObjects)
        {
            Destroy(obj);
        }
    }

    private void DestroyObject(int x, int y, int z)
    {
        GameObject objectToDelete = staticObjects[x, y, z];
        if (objectToDelete != null)
        {
            Destroy(objectToDelete);
        }
    }



    // Modify
    public void Modify(ID id, int x, int y, int z)
    {
        if (isClient) CmdModify(id, x, y, z);
    }

    [Command(ignoreAuthority = true)]
    private void CmdModify(ID id, int x, int y, int z)
    {
        levelIDs[Get1D(x, y, z)] = (short)id;
        RpcModify(id, x, y, z);
    }

    [ClientRpc]
    private void RpcModify(ID id, int x, int y, int z)
    {
        levelIDs[Get1D(x, y, z)] = (short)id;
        DestroyObject(x, y, z);
        SpawnObject(id, x, y, z);
    }



    // Utility
    public ID GetID(int x, int y, int z)
    {
        if (CheckIfInRange(x, y, z) == false) return ID.Empty;
        return (ID)levelIDs[Get1D(x, y, z)];
    }

    public GameObject GetStaticObject(int x, int y, int z)
    {
        if (CheckIfInRange(x, y, z) == false) return null;
        return staticObjects[x, y, z];
    }

    public bool CheckIfInRange(int x, int y, int z)
    {
        if (x >= 0 && x < GetLevelWidth() &&
            y >= 0 && y < GetLevelHeight() &&
            z >= 0 && z < GetLevelWidth())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int Get1D(int x, int y, int z) => x + (z * levelWidth) + (y * levelWidth * levelWidth);
}