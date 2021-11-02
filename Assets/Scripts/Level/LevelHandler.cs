using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : NetworkBehaviour
{
    private Level level;
    public Level GetLevel() => level;
    public void SetLevel(Level newLevel) => level = newLevel;

    private static LevelHandler singleton;
    private void Awake()
    {
        if (singleton != null) return;
        singleton = this;
    }

    public static void InitHostLevel()
    {
        //Level level = LevelUtil.LoadLevel(App.gameSaveName2);
        //if (level == null)
        //{
            Level level = LevelGenerator.Generate(8);
            HUD.LogMessage("LevelHandler: New Level Generated");
        //}
        singleton.SetLevel(level);
    }

    #region Send Level Data over Network

    public static void SendInitLevelInfo(NetworkConnection conn)
    {
        singleton.TargetSendInitLevelInfo(conn, singleton.GetLevel().levelWidthInChunks);
    }

    [TargetRpc] private void TargetSendInitLevelInfo(NetworkConnection conn, int levelWidthInChunks)
    {
        level = new Level(levelWidthInChunks);
    }

    public static void SendChunkToClient(NetworkConnection conn, int chunkX, int chunkY, int chunkZ)
    {
        singleton.TargetSendChunkToClient(conn, chunkX, chunkY, chunkZ, LevelUtil.CompressChunk(singleton.GetLevel().chunks[chunkX, chunkY, chunkZ]));
    }

    [TargetRpc] private void TargetSendChunkToClient(NetworkConnection target, int chunkX, int chunkY, int chunkZ, CompressedChunk compressedChunk)
    {
        level.chunks[chunkX, chunkY, chunkZ] = LevelUtil.UncompressChunk(compressedChunk);

        //DestroyAllStaticEntities();
        SpawnChunkObjects(chunkX, chunkY, chunkZ);
    }

    #endregion

    #region Spawn GameObjects
    private void SpawnChunkObjects(int chunkX, int chunkY, int chunkZ)
    {
        level.chunks[chunkX, chunkY, chunkZ].gameObjects = new GameObject[Chunk.width, Chunk.height, Chunk.width];

        for (int y = 0; y < Chunk.height; y++)
        {
            for (int z = 0; z < Chunk.width; z++)
            {
                for (int x = 0; x < Chunk.width; x++)
                {
                    SpawnObject(level.chunks[chunkX, chunkY, chunkZ].voxelIDs[x, y, z], chunkX, chunkY, chunkZ, x, y, z);
                }
            }
        }
    }

    private void SpawnObject(ID id, int chunkX, int chunkY, int chunkZ, int x, int y, int z)
    {
        if (id == ID.Empty) return;

        Entity entity = Entity.GetFromID(id);
        GameObject spawnedObj = Instantiate(entity.prefab, new Vector3(x + (chunkX * Chunk.width), y * 0.5f, z + (chunkZ * Chunk.width)), Quaternion.identity, transform);
        spawnedObj.GetComponent<Spawnable>().Init(entity.values);
        level.chunks[chunkX, chunkY, chunkZ].gameObjects[x, y, z] = spawnedObj;
    }
    #endregion
    
    #region Destroy GameObjects
    public void DestroyObjectsInAllChunks()
    {
        //foreach (GameObject obj in staticObjects)
        {
        //    Destroy(obj);
        }
    }

    private void DestroyObjectsInChunk(int chunkX, int chunkY, int chunkZ)
    {

    }

    private void DestroyObject(int chunkX, int chunkY, int chunkZ, int x, int y, int z)
    {
        //GameObject objectToDelete = staticObjects[x, y, z];
        //if (objectToDelete != null)
        {
        //    Destroy(objectToDelete);
        }
    }
    #endregion

    #region Modify Voxel
    // Change these to work with the new system (This should be left until last, once everything else works properly)
    public void ModifyVoxel(ID id, int x, int y, int z)
    {
        //if (isClient) CmdModifyVoxel(id, x, y, z);
    }

    [Command(ignoreAuthority = true)]
    private void CmdModifyVoxel(ID id, int x, int y, int z)
    {
        //levelIDs[Get1D(x, y, z)] = (short)id;
        //RpcModifyVoxel(id, x, y, z);
    }

    [ClientRpc]
    private void RpcModifyVoxel(ID id, int x, int y, int z)
    {
        //levelIDs[Get1D(x, y, z)] = (short)id;
        //DestroyObject(x, y, z);
        //SpawnObject(id, x, y, z);
    }
    #endregion

    #region Util Getters
    // These will need to be modified, repurposed, and moved around as needed to accompany the new system as it is being developed.
    //public ID GetID(int x, int y, int z)
    //{
        //if (CheckIfInRange(x, y, z) == false) return ID.Empty;
        //return (ID)levelIDs[Get1D(x, y, z)];
    //}

    //public GameObject GetStaticObject(int x, int y, int z)
   // {
        //if (CheckIfInRange(x, y, z) == false) return null;
        //return staticObjects[x, y, z];
   // }

    //public bool CheckIfInRange(int x, int y, int z)
    //{
       // if (x >= 0 && x < GetLevelWidth() &&
           // y >= 0 && y < GetLevelHeight() &&
            //z >= 0 && z < GetLevelWidth())
       // {
            //return true;
       // }
        //else
      //  {
            //return false;
       // }
    //}

    // The purpose of using a 1d array is because Mirror can not send a 2d or 3d array over the network
    // So 1d arrays only need to be used for compressed chunk info, live chunks should just use a regular 3d array
    //private int Get1D(int x, int y, int z) => x + (z * levelWidth) + (y * levelWidth * levelWidth);
    #endregion
}