using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class LevelSaveLoad
{
    /*
    public static GameSave Pack()
    {
        GameSave gameSave = new GameSave();

        gameSave.xWidth = ActiveGame.GetLevel().GetXWidth();
        gameSave.height = ActiveGame.GetLevel().GetHeight();
        gameSave.zWidth = ActiveGame.GetLevel().GetZWidth();
        gameSave.levelData = ActiveGame.GetLevel().GetLevelData();

        return gameSave;
    }

    public static void Save(GameSave gameSave, string filename)
    {
        string path = Application.persistentDataPath + "/" + filename + ".savegame";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(path);
        bf.Serialize(fileStream, gameSave);
        fileStream.Close();

        Log.LogMessage("File saved: " + path);
    }

    public static GameSave Load(string filename)
    {
        GameSave gameSave = new GameSave();
        string path = Application.persistentDataPath + "/" + filename + ".savegame";

        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(path, FileMode.Open);
            gameSave = (GameSave)bf.Deserialize(fileStream);
            fileStream.Close();

            gameSave.successfullyLoaded = true;
            Log.LogMessage("File loaded: " + path);
        }
        else
        {
            gameSave.successfullyLoaded = false;
            Log.LogError("Load Failed: File not found.");
        }

        return gameSave;
    }

    public static Level Unpack(GameSave gameSave)
    {
        Level level = new Level(gameSave.xWidth, gameSave.height, gameSave.zWidth);
        level.SetLevelData(gameSave.levelData);
        return level;
    }
    */
}