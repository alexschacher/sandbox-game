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