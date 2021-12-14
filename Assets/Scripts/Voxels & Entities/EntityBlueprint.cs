using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] public class EntityBlueprint : ScriptableObject
{
    public eID id;
    public GameObject prefab;
    public InitValues values;

    public static Dictionary<eID, EntityBlueprint> idLookup = new Dictionary<eID, EntityBlueprint>();
    public static EntityBlueprint GetFromID(eID id)
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
        EntityBlueprint[] allEntities = Resources.LoadAll<EntityBlueprint>("Entity");
        //Debug.Log("EntityInfo assets found: " + allEntities.Length);

        foreach (EntityBlueprint e in allEntities)
        {
            //Debug.Log(" - " + e.name);

            if (idLookup.ContainsKey(e.id))
            {
                throw new System.Exception("Error: Multiple scriptable objects registered to the same ID: " + e.id.ToString());
            }
            idLookup[e.id] = e;
        }

        //Debug.Log("EntityInfo assets loaded into idLookup: " + idLookup.Count);
    }
}

public enum eID : ushort
{
    Null,
    Apple,
    Log,
    Fish,
    Bobber
}