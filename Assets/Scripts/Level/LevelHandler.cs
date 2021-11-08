using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : NetworkBehaviour
{
    private Level level;
    public static Level GetLevel() => singleton.level;
    public static void SetLevel(Level newLevel)
    {
        Debug.Log("Set Level");
        singleton.level = newLevel;
    }

    private HashSet<Vector3Int> liveChunks = new HashSet<Vector3Int>();

    private static LevelHandler singleton;
    private void Awake()
    {
        if (singleton != null) return;
        singleton = this;
    }

    #region Init Level

    [Server]
    public static void InitHostLevel()
    {
        Debug.Log("InitHostLevel");
        Level level = LevelUtil.LoadLevel(App.GetGameSaveName());
        if (level == null)
        {
            level = LevelGenerator.Generate(16);
            HUD.LogMessage("LevelHandler: New Level Generated");
        }
        SetLevel(level);
    }

    [Server]
    public static void SendInitLevelInfo(NetworkConnection conn)
    {
        singleton.TargetSendInitLevelInfo(conn, GetLevel().levelWidthInChunks);
    }

    [TargetRpc]
    private void TargetSendInitLevelInfo(NetworkConnection conn, int levelWidthInChunks)
    {
        if (isServer == false)
        {
            SetLevel(new Level(levelWidthInChunks));
        }
    }

    #endregion

    [Server]
    public static void UpdateLiveChunks(List<GameObject> playerObjects)
    {
        int loadChunkZoneSize = 3;
        HashSet<Vector3Int> chunksThatShouldBeLive = new HashSet<Vector3Int>();
        HashSet<Vector3Int> liveChunksCopy = new HashSet<Vector3Int>(singleton.liveChunks);

        foreach (GameObject playerObj in playerObjects)
        {
            Vector3Int playerCoords = LevelUtil.GetChunkCoords(
                (int)playerObj.transform.position.x,
                (int)(playerObj.transform.position.y * 2f),
                (int)playerObj.transform.position.z);

            for (int x = -loadChunkZoneSize; x <= loadChunkZoneSize; x++)
            {
                for (int z = -loadChunkZoneSize; z <= loadChunkZoneSize; z++)
                {
                    chunksThatShouldBeLive.Add(playerCoords + new Vector3Int(x, 0, z));
                }
            }
        }

        HashSet<Vector3Int> chunkThatShouldBeLiveCopy = new HashSet<Vector3Int>(chunksThatShouldBeLive);

        int matchesFound = 0;
        foreach (Vector3Int chunkThatShouldBeLive in chunkThatShouldBeLiveCopy)
        {
            if (liveChunksCopy.Contains(chunkThatShouldBeLive))
            {
                matchesFound++;
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
            singleton.RpcUnloadChunkOnAllClients(chunkToUnload.x, chunkToUnload.y, chunkToUnload.z);
            singleton.liveChunks.Remove(chunkToUnload);
        }

        foreach (Vector3Int chunkToLoad in chunksThatShouldBeLive)
        {
            singleton.RpcSendChunkToAllClients(chunkToLoad.x, chunkToLoad.y, chunkToLoad.z, LevelUtil.CompressChunk(GetLevel().chunks[chunkToLoad.x, chunkToLoad.y, chunkToLoad.z]));
            singleton.liveChunks.Add(chunkToLoad);
        }
    }

    #region Send Chunk Data

    [Server]
    public static void SendAllLiveChunksToClient(NetworkConnection conn)
    {
        foreach (Vector3Int chunk in singleton.liveChunks)
        {
            SendChunkToClient(conn, chunk.x, chunk.y, chunk.z);
        }
    }

    [Server]
    public static void SendChunkToClient(NetworkConnection conn, int chunkX, int chunkY, int chunkZ)
    {
        singleton.TargetSendChunkToClient(conn, chunkX, chunkY, chunkZ, LevelUtil.CompressChunk(GetLevel().chunks[chunkX, chunkY, chunkZ]));
    }

    [ClientRpc] public void RpcUnloadChunkOnAllClients(int chunkX, int chunkY, int chunkZ) => DestroyObjectsInChunk(chunkX, chunkY, chunkZ);

    [ClientRpc] public void RpcSendChunkToAllClients(int chunkX, int chunkY, int chunkZ, CompressedChunk compressedChunk)
        => ReceiveCompressedChunk(chunkX, chunkY, chunkZ, compressedChunk);
    [TargetRpc] private void TargetSendChunkToClient(NetworkConnection target, int chunkX, int chunkY, int chunkZ, CompressedChunk compressedChunk)
        => ReceiveCompressedChunk(chunkX, chunkY, chunkZ, compressedChunk);
    
    private void ReceiveCompressedChunk(int chunkX, int chunkY, int chunkZ, CompressedChunk compressedChunk)
    {
        if (isServer == false)
        {
            level.chunks[chunkX, chunkY, chunkZ] = LevelUtil.UncompressChunk(compressedChunk);
        }
        DestroyObjectsInChunk(chunkX, chunkY, chunkZ);
        SpawnChunkObjects(chunkX, chunkY, chunkZ);
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

        if (level.chunks[chunkX, chunkY, chunkZ].gameObjects[x, y, z] != null)
        {
            Debug.Log("Uh Oh! Spawning object in place of a currently existing object! An orphan gameobject may be left over and unconnected to the level/chunk  array!");
        }

        Entity entity = Entity.GetFromID(id);
        GameObject spawnedObj = Instantiate(entity.prefab, new Vector3(x + (chunkX * Chunk.width), y * 0.5f, z + (chunkZ * Chunk.width)), Quaternion.identity, transform);
        spawnedObj.GetComponent<Spawnable>().Init(entity.values);
        level.chunks[chunkX, chunkY, chunkZ].gameObjects[x, y, z] = spawnedObj;
    }
    #endregion
    
    #region Destroy GameObjects
    public static void DestroyObjectsInAllChunks()
    {
        for (int y = 0; y < 1; y++)
        {
            for (int z = 0; z < singleton.level.levelWidthInChunks; z++)
            {
                for (int x = 0; x < singleton.level.levelWidthInChunks; x++)
                {
                    DestroyObjectsInChunk(x, y, z);
                }
            }
        }
    }

    private static void DestroyObjectsInChunk(int chunkX, int chunkY, int chunkZ)
    {
        for (int y = 0; y < Chunk.height; y++)
        {
            for (int z = 0; z < Chunk.width; z++)
            {
                for (int x = 0; x < Chunk.width; x++)
                {
                    DestroyObjectAtPosition(chunkX, chunkY, chunkZ, x, y, z);
                }
            }
        }
    }

    private static void DestroyObjectAtPosition(int chunkX, int chunkY, int chunkZ, int x, int y, int z)
    {
        GameObject objectToDelete = singleton.level.chunks[chunkX, chunkY, chunkZ].gameObjects[x, y, z];
        if (objectToDelete != null)
        {
            Destroy(objectToDelete);
            singleton.level.chunks[chunkX, chunkY, chunkZ].gameObjects[x, y, z] = null;
        }
    }
    #endregion

    #region Modify Voxel

    [Client]
    public static void ModifyVoxel(ID id, int worldX, int worldY, int worldZ)
    {
        //if (singleton.isClient) <-- Rendered useless by the [Client] tag (presumably?)
        //{
            singleton.CmdModifyVoxel(id, worldX, worldY, worldZ);
        //}
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