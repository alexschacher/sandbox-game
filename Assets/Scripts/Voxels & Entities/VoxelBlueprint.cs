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
                Debug.Log("Error: Multiple scriptable objects registered to the same ID: " + v.id.ToString());
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
    Cliff,
    Tallgrass,
    Gravestone,
    Cliff_Corner1,
    Cliff_Corner2,
    Cliff_Corner3,
    Cliff_Corner4,
    Cliff_Straight1,
    Cliff_Straight2,
    Cliff_Straight3,
    Cliff_Straight4,
    Cliff_InnerCorner1,
    Cliff_InnerCorner2,
    Cliff_InnerCorner3,
    Cliff_InnerCorner4,
    Bridge,
    Bridge_R90,
    Cliff_SlopeIn1,
    Cliff_SlopeIn2,
    Cliff_SlopeIn3,
    Cliff_SlopeIn4,
    Waterfall1,
    Waterfall2,
    Waterfall3,
    Waterfall4,
    Cliff_WaterfallCorner1,
    Cliff_WaterfallCorner2,
    Cliff_WaterfallCorner3,
    Cliff_WaterfallCorner4,
    UNUSED_2,
    UNUSED_3,
    UNUSED_4,
    UNUSED_5
}