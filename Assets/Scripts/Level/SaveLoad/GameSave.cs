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
}

[System.Serializable]
public class CompressedChunk
{
    public ID[] voxelIDs;
    public CompressedEntity[] entities;
}

[System.Serializable]
public class CompressedEntity
{
    public ID id;
    public float positionX;
    public float positionY;
    public float positionZ;
}

public class LiveLevel
{
    public int levelWidthInChunks;
    public LiveChunk[,,] liveChunks;
    public GameObject[] liveEntities;
}

public class LiveChunk
{
    public ID[,,] voxelIDs;
    public GameObject[,,] gameObjects;
}