using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Level : NetworkBehaviour
{
    private short[] levelData;
    private GameObject[,,] staticEntities;

    [SerializeField] private int levelWidth = 32;
    public int GetLevelWidth() => levelWidth;

    [SerializeField] private int levelHeight = 4;
    public int GetLevelHeight() => levelHeight;



    // Generate Level Data
    private void Start()
    {
        levelData = GenerateLevel();
        staticEntities = new GameObject[levelWidth, levelHeight, levelWidth];
    }

    private short[] GenerateLevel()
    {
        short[] levelData = new short[levelWidth * levelWidth * levelHeight];

        for (int z = 0; z < levelWidth; z++)
        {
            for (int x = 0; x < levelWidth; x++)
            {
                levelData[Get1D(x, 0, z)] = (short)ID.Ground;

                if (Random.Range(0f, 100f) < 5f)
                {
                    levelData[Get1D(x, 0, z)] = (short)ID.Water;
                }
                if (Random.Range(0f, 100f) < 3f)
                {
                    levelData[Get1D(x, 1, z)] = (short)ID.Apple;
                }
            }
        }
        return levelData;
    }



    // Send Level Data to Client
    private void OnGrab()
    {
        if (isServer)
        {
            SendLevelData(levelData);
        }
    }

    [ClientRpc]
    private void SendLevelData(short[] levelData)
    {
        this.levelData = levelData;
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
        staticEntities[x, y, z] = spawnedObj;
    }



    // Destroy Objects
    private void DestroyAllStaticEntities()
    {
        foreach (GameObject obj in staticEntities)
        {
            Destroy(obj);
        }
    }

    private void DestroyObject(int x, int y, int z)
    {
        GameObject objectToDelete = staticEntities[x, y, z];
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
        levelData[Get1D(x, y, z)] = (short)id;
        RpcModify(id, x, y, z);
    }

    [ClientRpc]
    private void RpcModify(ID id, int x, int y, int z)
    {
        levelData[Get1D(x, y, z)] = (short)id;
        DestroyObject(x, y, z);
        SpawnObject(id, x, y, z);
    }



    // Utility
    public ID GetCellID(int x, int y, int z)
    {
        return (ID)levelData[Get1D(x, y, z)];
    }

    private int Get1D(int x, int y, int z) => x + (z * levelWidth) + (y * levelWidth * levelWidth);
}