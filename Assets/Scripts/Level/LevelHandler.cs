using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : NetworkBehaviour
{
    private Level level;
    public static Level GetLevel() => singleton.level;
    public static void SetLevel(Level newLevel) => singleton.level = newLevel;

    private HashSet<Vector3Int> liveChunks = new HashSet<Vector3Int>();
    private List<GameObject> liveEntities = new List<GameObject>();

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
        Level level = LevelUtil.LoadLevel(App.GetGameSaveName());
        if (level == null)
        {
            level = LevelGenerator.Generate(64);
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
        int loadChunkZoneSize = 2;
        HashSet<Vector3Int> chunksThatShouldBeLive = new HashSet<Vector3Int>();
        HashSet<Vector3Int> liveChunksCopy = new HashSet<Vector3Int>(singleton.liveChunks);

        // Determine which chunks should currently be live
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

        // Determine chunks that need to be loaded and unloaded by comparing overlap in lists
        HashSet<Vector3Int> chunkThatShouldBeLiveCopy = new HashSet<Vector3Int>(chunksThatShouldBeLive);
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
            UnloadChunk(chunkToUnload);
        }
        foreach (Vector3Int chunkToLoad in chunksThatShouldBeLive)
        {
            LoadChunk(chunkToLoad);
        }
    }

    #region Load & Send Chunks

    [Server] public static void LoadChunk(Vector3Int chunkCoords)
    {
        singleton.RpcSendChunkToAllClients(chunkCoords.x, chunkCoords.y, chunkCoords.z, LevelUtil.CompressChunk(GetLevel().chunks[chunkCoords.x, chunkCoords.y, chunkCoords.z]));
        singleton.liveChunks.Add(chunkCoords);

        foreach (CompressedEntity compressedEntity in GetLevel().chunks[chunkCoords.x, chunkCoords.y, chunkCoords.z].storedEntities)
        {
            SpawnEntity((eID)compressedEntity.id, new Vector3(compressedEntity.positionX, compressedEntity.positionY, compressedEntity.positionZ));
        }
        GetLevel().chunks[chunkCoords.x, chunkCoords.y, chunkCoords.z].storedEntities.Clear();
    }

    [Server] public static void SendAllLiveChunksToClient(NetworkConnection conn)
    {
        foreach (Vector3Int chunk in singleton.liveChunks)
        {
            SendChunkToClient(conn, chunk.x, chunk.y, chunk.z);
        }
    }

    [Server] public static void SendChunkToClient(NetworkConnection conn, int chunkX, int chunkY, int chunkZ)
    {
        singleton.TargetSendChunkToClient(conn, chunkX, chunkY, chunkZ, LevelUtil.CompressChunk(GetLevel().chunks[chunkX, chunkY, chunkZ]));
    }

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
        DestroyVoxelObjectsInChunk(chunkX, chunkY, chunkZ);
        InstantiateChunkVoxelObjects(chunkX, chunkY, chunkZ);
    }

    #endregion

    #region Unload Chunks

    [Server] public static void UnloadAllChunks()
    {
        HashSet<Vector3Int> liveChunksCopy = new HashSet<Vector3Int>(singleton.liveChunks);
        foreach(Vector3Int chunkCoords in liveChunksCopy)
        {
            UnloadChunk(chunkCoords);
        }
    }

    [Server] public static void UnloadChunk(Vector3Int chunkCoords)
    {
        List<GameObject> liveEntitiesToRemove = new List<GameObject>();

        singleton.RpcUnloadChunkOnAllClients(chunkCoords.x, chunkCoords.y, chunkCoords.z);
        singleton.liveChunks.Remove(chunkCoords);

        foreach (GameObject liveEntity in singleton.liveEntities)
        {
            if (LevelUtil.GetChunkCoords((int)liveEntity.transform.position.x, (int)liveEntity.transform.position.y, (int)liveEntity.transform.position.z).Equals(chunkCoords))
            {
                DespawnEntity(liveEntity, chunkCoords);
                liveEntitiesToRemove.Add(liveEntity);
            }
        }
        foreach (GameObject liveEntity in liveEntitiesToRemove)
        {
            singleton.liveEntities.Remove(liveEntity);
        }
    }

    [ClientRpc] public void RpcUnloadChunkOnAllClients(int chunkX, int chunkY, int chunkZ) => DestroyVoxelObjectsInChunk(chunkX, chunkY, chunkZ);

    #endregion

    #region Spawn & Despawn Entities

    [Server]
    public static void SpawnEntity(eID id, Vector3 worldPosition)
    {
        EntityBlueprint entityBP = EntityBlueprint.GetFromID(id);
        GameObject entityObj = Instantiate(entityBP.prefab, worldPosition, Quaternion.identity, singleton.transform);
        entityObj.GetComponent<EntityObject>().ServerInitEntity(entityBP.id);
        NetworkServer.Spawn(entityObj);
        singleton.liveEntities.Add(entityObj);
    }

    [Server]
    public static void DespawnEntity(GameObject liveEntity, Vector3Int chunkCoords)
    {
        EntityObject entityObject = liveEntity.GetComponent<EntityObject>();

        CompressedEntity compressedEntity = new CompressedEntity();
        compressedEntity.id = (ushort)entityObject.GetID();
        compressedEntity.positionX = liveEntity.transform.position.x;
        compressedEntity.positionY = liveEntity.transform.position.y;
        compressedEntity.positionZ = liveEntity.transform.position.z;
        GetLevel().chunks[chunkCoords.x, chunkCoords.y, chunkCoords.z].storedEntities.Add(compressedEntity);

        NetworkServer.Destroy(liveEntity);
    }

    #endregion

    #region Instantiate Voxel Objects

    private void InstantiateChunkVoxelObjects(int chunkX, int chunkY, int chunkZ)
    {
        for (int y = 0; y < Chunk.height; y++)
        {
            for (int z = 0; z < Chunk.width; z++)
            {
                for (int x = 0; x < Chunk.width; x++)
                {
                    InstantiateVoxelObject(level.chunks[chunkX, chunkY, chunkZ].voxelIDs[x, y, z], chunkX, chunkY, chunkZ, x, y, z);
                }
            }
        }
    }

    private void InstantiateVoxelObject(vID id, int chunkX, int chunkY, int chunkZ, int x, int y, int z)
    {
        if (id == vID.Empty) return;

        VoxelBlueprint voxelBP = VoxelBlueprint.GetFromID(id);
        GameObject voxelObj = Instantiate(voxelBP.prefab, new Vector3(x + (chunkX * Chunk.width), y * 0.5f, z + (chunkZ * Chunk.width)), Quaternion.identity, transform);
        voxelObj.GetComponent<VoxelObject>().Init(voxelBP.values);
        level.chunks[chunkX, chunkY, chunkZ].gameObjects[x, y, z] = voxelObj;
    }
    #endregion

    #region Destroy Voxel Objects
    public static void DestroyVoxelObjectsInAllChunks()
    {
        for (int y = 0; y < 1; y++)
        {
            for (int z = 0; z < singleton.level.levelWidthInChunks; z++)
            {
                for (int x = 0; x < singleton.level.levelWidthInChunks; x++)
                {
                    DestroyVoxelObjectsInChunk(x, y, z);
                }
            }
        }
    }

    private static void DestroyVoxelObjectsInChunk(int chunkX, int chunkY, int chunkZ)
    {
        for (int y = 0; y < Chunk.height; y++)
        {
            for (int z = 0; z < Chunk.width; z++)
            {
                for (int x = 0; x < Chunk.width; x++)
                {
                    DestroyVoxelObjectAtPosition(chunkX, chunkY, chunkZ, x, y, z);
                }
            }
        }
    }

    private static void DestroyVoxelObjectAtPosition(int chunkX, int chunkY, int chunkZ, int x, int y, int z)
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
    public static void ModifyVoxel(vID id, int worldX, int worldY, int worldZ)
    {
        singleton.CmdModifyVoxel(id, worldX, worldY, worldZ);
    }

    [Command(ignoreAuthority = true)]
    private void CmdModifyVoxel(vID id, int worldX, int worldY, int worldZ)
    {
        Vector3Int chunkCoords = LevelUtil.GetChunkCoords(worldX, worldY, worldZ);
        Vector3Int voxelCoords = LevelUtil.GetVoxelCoords(worldX, worldY, worldZ);

        level.chunks[chunkCoords.x, chunkCoords.y, chunkCoords.z].voxelIDs[voxelCoords.x, voxelCoords.y, voxelCoords.z] = id;

        RpcModifyVoxel(id, worldX, worldY, worldZ);
    }

    [ClientRpc]
    private void RpcModifyVoxel(vID id, int worldX, int worldY, int worldZ)
    {
        Vector3Int chunkCoords = LevelUtil.GetChunkCoords(worldX, worldY, worldZ);
        Vector3Int voxelCoords = LevelUtil.GetVoxelCoords(worldX, worldY, worldZ);

        level.chunks[chunkCoords.x, chunkCoords.y, chunkCoords.z].voxelIDs[voxelCoords.x, voxelCoords.y, voxelCoords.z] = id;

        DestroyVoxelObjectAtPosition(chunkCoords.x, chunkCoords.y, chunkCoords.z, voxelCoords.x, voxelCoords.y, voxelCoords.z);

        InstantiateVoxelObject(id, chunkCoords.x, chunkCoords.y, chunkCoords.z, voxelCoords.x, voxelCoords.y, voxelCoords.z);
    }
    #endregion

    #region Util Getters

    public static vID GetVoxelIdAtPosition(int worldX, int worldY, int worldZ)
    {
        if (CheckIfInRange(worldX, worldY, worldZ) == false) return vID.Empty;

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