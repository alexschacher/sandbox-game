[System.Serializable]
public class GameSave
{
    public bool successfullyLoaded = false;
    public int xWidth;
    public int height;
    public int zWidth;
    public ID[,,] levelData;
}