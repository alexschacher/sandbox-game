using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Level_Old : NetworkBehaviour
{

    // This will all be replaced by private LiveLevel liveLevel, and appropriate getters, setters, and initializers
    private short[] levelIDs;
    public short[] GetLevelData() => levelIDs;
    private GameObject[,,] staticObjects;

    [SerializeField] private int levelWidth = 32;
    public int GetLevelWidth() => levelWidth;

    [SerializeField] private int levelHeight = 4;
    public int GetLevelHeight() => levelHeight;

    public void SetLevelData(int width, int height, short[] levelData)
    {
        levelWidth = width;
        levelHeight = height;
        levelIDs = levelData;
    }

    private void Awake()
    {
        staticObjects = new GameObject[levelWidth, levelHeight, levelWidth];
    }
    













    // This should be included in its own LevelGenerator class
    // This generation will generate a world in the LiveLevel format, without using the Gameobject holders
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












    // Everything below should be a LevelHandler class which holds a Level and modifies it over the network etc




    // These should be replaced by a message of SendChunkDataToClient, which just sets a chunk within the clients LiveLevel to new data
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







    // SpawnLevel is replaced by SpawnObjectsInChunk, and SpawnObject places its gameObject in the new correct place
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











    // This is modified to include a DestroyObjectsInChunk
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










    // Change these to work with the new system (This should be left until last, once everything else works properly)
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













    // These will need to be modified, repurposed, and moved around as needed to accompany the new system as it is being developed.
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

    // The purpose of using a 1d array is because Mirror can not send a 2d or 3d array over the network
    // So 1d arrays only need to be used for compressed chunk info, live chunks should just use a regular 3d array
    private int Get1D(int x, int y, int z) => x + (z * levelWidth) + (y * levelWidth * levelWidth);
}