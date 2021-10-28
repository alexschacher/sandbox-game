using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class LevelSaveLoad
{
    public static void Save(Level level, string filename)
    {
        string path = Application.persistentDataPath + "/" + filename + ".savegame";

        GameSave gameSave = new GameSave();
        gameSave.compressedLevel = new CompressedLevel(level.levelWidthInChunks);

        for (int x = 0; x < level.levelWidthInChunks; x++)
        {
            for (int z = 0; z < level.levelWidthInChunks; z++)
            {
                gameSave.compressedLevel.compressedChunks[x, 0, z] = CompressChunk(level.chunks[x, 0, z]);
            }
        }
    }

    public static Level Load(string filename)
    {
        Level level = new Level(1);
        return level;
    }

    public static CompressedChunk CompressChunk(Chunk chunk)
    {
        List<short> dataList = new List<short>();

        short thisID;
        short lastID = CompressedChunk.nullSign;
        short repeatCount = 0;

        for (int y = 0; y < Chunk.height; y++)
        {
            for (int x = 0; x < Chunk.width; x++)
            {
                for (int z = 0; z < Chunk.width; z++)
                {
                    thisID = (short)chunk.voxelIDs[x, y, z];

                    if (thisID == lastID)
                    {
                        repeatCount++;
                    }
                    else
                    {
                        if (repeatCount > 0)
                        {
                            if (repeatCount == 1)
                            {
                                dataList.Add(lastID);
                            }
                            else
                            {
                                dataList.Add(CompressedChunk.repeatSign);
                                dataList.Add(repeatCount);
                                repeatCount = 0;
                            }
                        }
                        dataList.Add(thisID);
                    }
                    lastID = thisID;
                }
            }
        }

        if (repeatCount > 0)
        {
            dataList.Add(CompressedChunk.repeatSign);
            dataList.Add(repeatCount);
        }

        CompressedChunk compressedChunk = new CompressedChunk();
        compressedChunk.compressedVoxelData = new short[dataList.Count];
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

        for (int i = 0; i < compressedChunk.compressedVoxelData.Length - 1; i++)
        {
            uncompressedChunk.voxelIDs[x, y, z] = (ID)compressedChunk.compressedVoxelData[i];

            if (compressedChunk.compressedVoxelData[i + 1] == CompressedChunk.repeatSign)
            {
                for (int r = 0; r < compressedChunk.compressedVoxelData[i + 2] - 1; r++)
                {
                    z++; if (z > Chunk.width - 1) { z = 0; x++; if (x > Chunk.width - 1) { x = 0; y++; } }
                    uncompressedChunk.voxelIDs[x, y, z] = (ID)compressedChunk.compressedVoxelData[i];
                }
                i += 3;
            }

            z++; if (z > Chunk.width - 1) { z = 0; x++; if (x > Chunk.width - 1) { x = 0; y++; } }
        }
        return uncompressedChunk;
    }






    // Old Format

    public static void Save(string filename)
    {
        string path = Application.persistentDataPath + "/" + filename + ".savegame";

        GameSave gameSave = new GameSave();
        gameSave.xWidth = App.GetLevel().GetLevelWidth();
        gameSave.height = App.GetLevel().GetLevelHeight();
        gameSave.zWidth = App.GetLevel().GetLevelWidth();
        gameSave.levelData = App.GetLevel().GetLevelData();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(path);
        bf.Serialize(fileStream, gameSave);
        fileStream.Close();

        HUD.LogMessage("Level saved: " + path);
    }

    public static bool LoadLevel(string filename)
    {
        string path = Application.persistentDataPath + "/" + filename + ".savegame";
        GameSave gameSave = new GameSave();

        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(path, FileMode.Open);
            gameSave = (GameSave)bf.Deserialize(fileStream);
            fileStream.Close();

            App.GetLevel().SetLevelData(gameSave.xWidth, gameSave.height, gameSave.levelData);

            gameSave.successfullyLoaded = true;
            HUD.LogMessage("Level loaded: " + path);
        }
        else
        {
            gameSave.successfullyLoaded = false;
            HUD.LogMessage("Level Load Failed: File not found.");
        }
        return gameSave.successfullyLoaded;
    }
}