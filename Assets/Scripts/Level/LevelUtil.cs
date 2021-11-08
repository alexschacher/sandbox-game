using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class LevelUtil
{
    public static void SaveLevel(Level level, string filename)
    {
        string path = Application.persistentDataPath + "/" + filename + ".savegame";

        GameSave gameSave = new GameSave();
        gameSave.compressedLevel = CompressLevel(level);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(path);
        bf.Serialize(fileStream, gameSave);
        fileStream.Close();

        HUD.LogMessage("Level saved: " + path);
    }

    public static Level LoadLevel(string filename)
    {
        string path = Application.persistentDataPath + "/" + filename + ".savegame";

        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(path, FileMode.Open);
            GameSave gameSave = (GameSave)bf.Deserialize(fileStream);
            fileStream.Close();

            Level level = UncompressLevel(gameSave.compressedLevel);

            HUD.LogMessage("Level loaded: " + path);
            return level;
        }
        else
        {
            HUD.LogMessage("Level Load Failed: File not found.");
            return null;
        }
    }

    public static CompressedLevel CompressLevel(Level level)
    {
        CompressedLevel compressedLevel = new CompressedLevel(level.levelWidthInChunks);

        for (int x = 0; x < level.levelWidthInChunks; x++)
        {
            for (int z = 0; z < level.levelWidthInChunks; z++)
            {
                compressedLevel.compressedChunks[x, 0, z] = CompressChunk(level.chunks[x, 0, z]);
            }
        }
        return compressedLevel;
    }

    public static Level UncompressLevel(CompressedLevel compressedLevel)
    {
        Level level = new Level(compressedLevel.levelWidthInChunks);

        for (int x = 0; x < level.levelWidthInChunks; x++)
        {
            for (int z = 0; z < level.levelWidthInChunks; z++)
            {
                level.chunks[x, 0, z] = UncompressChunk(compressedLevel.compressedChunks[x, 0, z]);
            }
        }
        return level;
    }

    public static CompressedChunk CompressChunk(Chunk chunk)
    {
        List<ushort> dataList = new List<ushort>();

        ushort thisID;
        ushort lastID = CompressedChunk.nullSign;
        ushort repeatCount = 0;

        for (int y = 0; y < Chunk.height; y++)
        {
            for (int x = 0; x < Chunk.width; x++)
            {
                for (int z = 0; z < Chunk.width; z++)
                {
                    thisID = (ushort)chunk.voxelIDs[x, y, z];

                    if (thisID == lastID)
                    {
                        repeatCount++;
                    }
                    else
                    {
                        if (repeatCount > 0)
                        {
                            dataList.Add((ushort)(CompressedChunk.repeatBase + repeatCount));
                            repeatCount = 0;
                        }
                        dataList.Add(thisID);
                    }
                    lastID = thisID;
                }
            }
        }

        if (repeatCount > 0)
        {
            dataList.Add((ushort)(CompressedChunk.repeatBase + repeatCount));
        }

        CompressedChunk compressedChunk = new CompressedChunk();
        compressedChunk.compressedVoxelData = new ushort[dataList.Count];

        for (int i = 0; i < dataList.Count; i++)
        {
            compressedChunk.compressedVoxelData[i] = dataList[i];
        }
        return compressedChunk;
    }

    public static Chunk UncompressChunk(CompressedChunk compressedChunk)
    {
        Chunk uncompressedChunk = new Chunk();

        int x = 0, y = 0, z = 0;

        for (int i = 0; i < compressedChunk.compressedVoxelData.Length; i++)
        {
            if (compressedChunk.compressedVoxelData[i] > CompressedChunk.repeatBase) continue;

            uncompressedChunk.voxelIDs[x, y, z] = (ID)compressedChunk.compressedVoxelData[i];

            if (compressedChunk.compressedVoxelData[i + 1] > CompressedChunk.repeatBase)
            {
                int repeat = compressedChunk.compressedVoxelData[i + 1] - CompressedChunk.repeatBase;

                for (int r = 0; r < repeat; r++)
                {
                    z++; if (z > Chunk.width - 1) { z = 0; x++; if (x > Chunk.width - 1) { x = 0; y++; if (y > Chunk.height - 1) { Debug.Log("Uncompress Chunk Space Overflow"); break; } } }

                    uncompressedChunk.voxelIDs[x, y, z] = (ID)compressedChunk.compressedVoxelData[i];
                }
            }
            z++; if (z > Chunk.width - 1) { z = 0; x++; if (x > Chunk.width - 1) { x = 0; y++; if (y > Chunk.height - 1) { break; } } }
        }
        return uncompressedChunk;
    }

    public static Vector3Int GetChunkCoords(int worldX, int worldY, int worldZ)
    {
        return new Vector3Int(Mathf.FloorToInt(worldX / Chunk.width), Mathf.FloorToInt(worldY / Chunk.height), Mathf.FloorToInt(worldZ / Chunk.width));
    }

    public static Vector3Int GetVoxelCoords(int worldX, int worldY, int worldZ)
    {
        return new Vector3Int(worldX % Chunk.width, worldY % Chunk.height, worldZ % Chunk.width);
    }
}

[System.Serializable]
public class GameSave
{
    public bool successfullyLoaded = false;
    public int xWidth; // Not needed in new format, delete when done
    public int height; // Not needed in new format, delete when done
    public int zWidth; // Not needed in new format, delete when done
    public short[] levelData; // Not needed in new format, delete when done
    public CompressedLevel compressedLevel;
}

[System.Serializable]
public class CompressedLevel
{
    public int levelWidthInChunks;
    public CompressedChunk[,,] compressedChunks;

    public CompressedLevel(int width)
    {
        levelWidthInChunks = width;
        compressedChunks = new CompressedChunk[width, 1, width];
    }
}

[System.Serializable]
public class CompressedChunk
{
    public static readonly ushort repeatBase = 65000;
    public static readonly ushort nullSign = 65535;
    public ushort[] compressedVoxelData;
    public CompressedEntity[] entities;
}

[System.Serializable]
public class CompressedEntity
{
    public ushort id;
    public float positionX;
    public float positionY;
    public float positionZ;
}

public class Level
{
    public int levelWidthInChunks;
    public Chunk[,,] chunks;
    public GameObject[] entities;

    public Level(int width)
    {
        levelWidthInChunks = width;
        chunks = new Chunk[width, 1, width];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                chunks[x, 0, z] = new Chunk();
            }
        }
    }
}

public class Chunk
{
    public static readonly int width = 8;
    public static readonly int height = 8;
    public ID[,,] voxelIDs;
    public GameObject[,,] gameObjects;

    public Chunk()
    {
        voxelIDs = new ID[width, height, width];
        gameObjects = new GameObject[width, height, width];
    }
}