using UnityEngine;


// Rename this file to something more appropriate


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
    public static readonly short repeatSign = 32767;
    public static readonly short nullSign = 32766;
    public short[] compressedVoxelData;
    public CompressedEntity[] entities;
}

[System.Serializable]
public class CompressedEntity
{
    public short id;
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
    }
}