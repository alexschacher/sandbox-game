using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : NetworkBehaviour
{
    private Level level;
    public static Level GetLevel() => singleton.level;
    public static void SetLevel(Level newLevel) => singleton.level = newLevel;

    private List<Vector3Int> liveChunks = new List<Vector3Int>();

    private static LevelHandler singleton;
    private void Awake()
    {
        if (singleton != null) return;
        singleton = this;
    }

    public static void InitHostLevel()
    {
        Debug.Log("InitHostLevel");
        //Level level = LevelUtil.LoadLevel(App.GetGameSaveName());
        //if (level == null)
        //{
            Level level = LevelGenerator.Generate(8);
            HUD.LogMessage("LevelHandler: New Level Generated");
        //}
        SetLevel(level);
    }

    public static void UpdateLiveChunks(List<GameObject> playerObjects)
    {
        Debug.Log("Update Live Chunks");

        List<Vector3Int> chunksThatShouldBeLive = new List<Vector3Int>();
        List<Vector3Int> liveChunksCopy = singleton.liveChunks;

        foreach (GameObject playerObj in playerObjects)
        {
            Vector3Int playerCoords = LevelUtil.GetChunkCoords(
                (int)playerObj.transform.position.x,
                (int)(playerObj.transform.position.y * 2f),
                (int)playerObj.transform.position.z);

            chunksThatShouldBeLive.Add(playerCoords);
            chunksThatShouldBeLive.Add(playerCoords + new Vector3Int(1, 0, 0));
            chunksThatShouldBeLive.Add(playerCoords + new Vector3Int(-1, 0, 0));
            chunksThatShouldBeLive.Add(playerCoords + new Vector3Int(1, 0, 1));
            chunksThatShouldBeLive.Add(playerCoords + new Vector3Int(-1, 0, 1));
            chunksThatShouldBeLive.Add(playerCoords + new Vector3Int(1, 0, -1));
            chunksThatShouldBeLive.Add(playerCoords + new Vector3Int(-1, 0, -1));
            chunksThatShouldBeLive.Add(playerCoords + new Vector3Int(0, 0, 1));
            chunksThatShouldBeLive.Add(playerCoords + new Vector3Int(0, 0, -1));
        }

        List<Vector3Int> chunkThatShouldBeLiveCopy = new List<Vector3Int>(chunksThatShouldBeLive);
        foreach (Vector3Int chunkThatShouldBeLive in chunkThatShouldBeLiveCopy)
        {
            if (liveChunksCopy.Contains(chunkThatShouldBeLive))
            {
                chunksThatShouldBeLive.Remove(chunkThatShouldBeLive);
                liveChunksCopy.Remove(chunkThatShouldBeLive);
            }
            else
            {
                if (chunkThatShouldBeLive.x < 0 || chunkThatShouldBeLive.z < 0 ||
                    chunkThatShouldBeLive.x > singleton.level.levelWidthInChunks - 1 ||
                    chunkThatShouldBeLive.z > singleton.level.levelWidthInChunks - 1)
                {
                    chunksThatShouldBeLive.Remove(chunkThatShouldBeLive);
                }
            }
        } 

        foreach (Vector3Int chunkToUnload in liveChunksCopy)
        {
            Debug.Log("Unload chunk: " + chunkToUnload.x + " " + chunkToUnload.y + " " + chunkToUnload.z);
            singleton.RpcUnloadChunk(chunkToUnload.x, chunkToUnload.y, chunkToUnload.z);
            singleton.liveChunks.Remove(chunkToUnload);
        }

        foreach (Vector3Int chunkToLoad in chunksThatShouldBeLive)
        {
            Debug.Log("Load chunk: " + chunkToLoad.x + " " + chunkToLoad.y + " " + chunkToLoad.z);
            singleton.RpcSendChunk(chunkToLoad.x, chunkToLoad.y, chunkToLoad.z, LevelUtil.CompressChunk(singleton.level.chunks[chunkToLoad.x, chunkToLoad.y, chunkToLoad.z]));
            singleton.liveChunks.Add(chunkToLoad);
        }
    }

    #region Send Level Data over Network

    public static void SendInitLevelInfo(NetworkConnection conn)
    {
        singleton.TargetSendInitLevelInfo(conn, GetLevel().levelWidthInChunks);
    }

    [TargetRpc] private void TargetSendInitLevelInfo(NetworkConnection conn, int levelWidthInChunks)
    {
        level = new Level(levelWidthInChunks);
    }

    public static void SendAllLiveChunksToClient(NetworkConnection conn)
    {
        foreach (Vector3Int chunk in singleton.liveChunks)
        {
            SendChunkToClient(conn, chunk.x, chunk.y, chunk.z);
        }
    }

    public static void SendChunkToClient(NetworkConnection conn, int chunkX, int chunkY, int chunkZ)
    {
        singleton.TargetSendChunkToClient(conn, chunkX, chunkY, chunkZ, LevelUtil.CompressChunk(GetLevel().chunks[chunkX, chunkY, chunkZ]));
    }

    [TargetRpc] private void TargetSendChunkToClient(NetworkConnection target, int chunkX, int chunkY, int chunkZ, CompressedChunk compressedChunk)
    {
        level.chunks[chunkX, chunkY, chunkZ] = LevelUtil.UncompressChunk(compressedChunk);

        DestroyObjectsInChunk(chunkX, chunkY, chunkZ);
        SpawnChunkObjects(chunkX, chunkY, chunkZ);
    }

    [ClientRpc] public void RpcSendChunk(int chunkX, int chunkY, int chunkZ, CompressedChunk compressedChunk)
    {
        string message = "Rpc Send Chunk: ";
        foreach (ushort u in compressedChunk.compressedVoxelData)
        {
            message = message + u + " ";
        }
        Debug.Log(message);

        singleton.level.chunks[chunkX, chunkY, chunkZ] = LevelUtil.UncompressChunk(compressedChunk);

        DestroyObjectsInChunk(chunkX, chunkY, chunkZ);
        singleton.SpawnChunkObjects(chunkX, chunkY, chunkZ);
    }

    [ClientRpc] public void RpcUnloadChunk(int chunkX, int chunkY, int chunkZ)
    {
        DestroyObjectsInChunk(chunkX, chunkY, chunkZ);
    }

    #endregion

    #region Spawn GameObjects
    private void SpawnChunkObjects(int chunkX, int chunkY, int chunkZ)
    {
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
    public static void DestroyObjectsInAllChunks()
    {
        foreach (Chunk chunk in singleton.level.chunks)
        {
            foreach (GameObject obj in chunk.gameObjects)
            {
                Destroy(obj);
            }
        }
    }

    private static void DestroyObjectsInChunk(int chunkX, int chunkY, int chunkZ)
    {
        foreach(GameObject obj in singleton.level.chunks[chunkX, chunkY, chunkZ].gameObjects)
        {
            Destroy(obj);
        }
    }

    private static void DestroyObjectAtPosition(int chunkX, int chunkY, int chunkZ, int x, int y, int z)
    {
        GameObject objectToDelete = singleton.level.chunks[chunkX, chunkY, chunkZ].gameObjects[x, y, z];
        if (objectToDelete != null)
        {
            Destroy(objectToDelete);
        }
    }
    #endregion

    #region Modify Voxel

    public static void ModifyVoxel(ID id, int worldX, int worldY, int worldZ)
    {
        if (singleton.isClient)
        {
            singleton.CmdModifyVoxel(id, worldX, worldY, worldZ);
        }
    }

    [Command(ignoreAuthority = true)]
    private void CmdModifyVoxel(ID id, int worldX, int worldY, int worldZ)
    {
        Vector3Int chunkCoords = LevelUtil.GetChunkCoords(worldX, worldY, worldZ);
        Vector3Int voxelCoords = LevelUtil.GetVoxelCoords(worldX, worldY, worldZ);

        level.chunks[chunkCoords.x, chunkCoords.y, chunkCoords.z].voxelIDs[voxelCoords.x, voxelCoords.y, voxelCoords.z] = id;

        RpcModifyVoxel(id, worldX, worldY, worldZ);
    }

    [ClientRpc]
    private void RpcModifyVoxel(ID id, int worldX, int worldY, int worldZ)
    {
        Vector3Int chunkCoords = LevelUtil.GetChunkCoords(worldX, worldY, worldZ);
        Vector3Int voxelCoords = LevelUtil.GetVoxelCoords(worldX, worldY, worldZ);

        level.chunks[chunkCoords.x, chunkCoords.y, chunkCoords.z].voxelIDs[voxelCoords.x, voxelCoords.y, voxelCoords.z] = id;

        DestroyObjectAtPosition(chunkCoords.x, chunkCoords.y, chunkCoords.z, voxelCoords.x, voxelCoords.y, voxelCoords.z);

        SpawnObject(id, chunkCoords.x, chunkCoords.y, chunkCoords.z, voxelCoords.x, voxelCoords.y, voxelCoords.z);
    }
    #endregion

    #region Util Getters

    public static ID GetIdAtPosition(int worldX, int worldY, int worldZ)
    {
        if (CheckIfInRange(worldX, worldY, worldZ) == false) return ID.Empty;

        Vector3Int chunkCoords = LevelUtil.GetChunkCoords(worldX, worldY, worldZ);
        Vector3Int voxelCoords = LevelUtil.GetVoxelCoords(worldX, worldY, worldZ);

        return singleton.level.chunks[chunkCoords.x, chunkCoords.y, chunkCoords.z].voxelIDs[voxelCoords.x, voxelCoords.y, voxelCoords.z];
    }

    public static GameObject GetObjectAtPosition(int worldX, int worldY, int worldZ)
    {
        if (CheckIfInRange(worldX, worldY, worldZ) == false) return null;

        Vector3Int chunkCoords = LevelUtil.GetChunkCoords(worldX, worldY, worldZ);
        Vector3Int voxelCoords = LevelUtil.GetVoxelCoords(worldX, worldY, worldZ);

        return singleton.level.chunks[chunkCoords.x, chunkCoords.y, chunkCoords.z].gameObjects[voxelCoords.x, voxelCoords.y, voxelCoords.z];
    }

    public static bool CheckIfInRange(int x, int y, int z)
    {
        if (x >= 0 && x < GetLevel().levelWidthInChunks * Chunk.width &&
        y >= 0 && y < Chunk.height &&
        z >= 0 && z < GetLevel().levelWidthInChunks * Chunk.width)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
}