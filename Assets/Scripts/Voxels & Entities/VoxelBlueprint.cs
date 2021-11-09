using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] public class VoxelBlueprint : ScriptableObject
{
    public vID id;
    public GameObject prefab;
    public InitValues values;

    public static Dictionary<vID, VoxelBlueprint> idLookup = new Dictionary<vID, VoxelBlueprint>();
    public static VoxelBlueprint GetFromID(vID id)
    {
        if (idLookup.ContainsKey(id))
        {
            return idLookup[id];
        }
        Debug.Log("Trying to access non-existant entity ID");
        return null;
    }
    public static void InitDictionary()
    {
        VoxelBlueprint[] allEntities = Resources.LoadAll<VoxelBlueprint>("Entity");
        //Debug.Log("EntityInfo assets found: " + allEntities.Length);

        foreach (VoxelBlueprint v in allEntities)
        {
            //Debug.Log(" - " + e.name);

            if (idLookup.ContainsKey(v.id))
            {
                throw new System.Exception("Error: Multiple scriptable objects registered to the same ID: " + v.id.ToString());
            }
            idLookup[v.id] = v;
        }

        //Debug.Log("EntityInfo assets loaded into idLookup: " + idLookup.Count);
    }
}

public enum vID : ushort
{
    Empty,
    Ground,
    Water,
    Post,
    Tree,
    Bridge,
    Tallgrass,
    Gravestone
}