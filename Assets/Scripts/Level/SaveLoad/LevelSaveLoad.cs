using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class LevelSaveLoad
{
    public static void Save(string filename)
    {
        string path = Application.persistentDataPath + "/" + filename + ".savegame";

        GameSave gameSave = new GameSave();
        gameSave.xWidth = App.GetLevel().GetLevelWidth();
        gameSave.height = App.GetLevel().GetLevelHeight();
        gameSave.zWidth = App.GetLevel().GetLevelWidth();
        gameSave.levelData = App.GetLevel().GetLevelData();










        // Testing new format
        ID[] testVoxelIDs = new ID[10];
        for (int i = 0; i < 10; i++)
        {
            testVoxelIDs[i] = ID.Ground;
        }

        CompressedEntity[] testChunkEntities = new CompressedEntity[3];
        for (int i = 0; i < 3; i++)
        {
            testChunkEntities[i] = new CompressedEntity();
            testChunkEntities[i].id = ID.Apple;
            testChunkEntities[i].positionX = 0.1f;
            testChunkEntities[i].positionY = 10.34f;
            testChunkEntities[i].positionZ = 102.432f;
        }

        CompressedLevel compressedLevel = new CompressedLevel();
        compressedLevel.levelWidthInChunks = 4;
        compressedLevel.compressedChunks = new CompressedChunk[4,4,4];
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    compressedLevel.compressedChunks[x,y,z] = new CompressedChunk();
                    compressedLevel.compressedChunks[x, y, z].voxelIDs = testVoxelIDs;
                    compressedLevel.compressedChunks[x, y, z].entities = testChunkEntities;
                }
            }
        }
        gameSave.compressedLevel = compressedLevel; 
        // End Test












        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(path);
        bf.Serialize(fileStream, gameSave);
        fileStream.Close();

        HUD.LogMessage("Level saved: " + path);
    }


























    public static bool Load(string filename)
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













            // Testing new format
            CompressedLevel compressedLevel = gameSave.compressedLevel;

            Debug.Log("Test new save format begin");
            Debug.Log(compressedLevel.levelWidthInChunks);
            Debug.Log(compressedLevel.compressedChunks[2, 2, 2].entities[2].id);
            Debug.Log(compressedLevel.compressedChunks[2, 2, 2].entities[2].positionZ);
            Debug.Log(compressedLevel.compressedChunks[2, 2, 2].voxelIDs[5]);
            Debug.Log("Test new save format end");
            // End Test












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