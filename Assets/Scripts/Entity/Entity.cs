using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] public class Entity : ScriptableObject
{
    public ID id;
    public GameObject prefab;
    public InitValues values;

    public static Dictionary<ID, Entity> idLookup = new Dictionary<ID, Entity>();
    public static Entity GetFromID(ID id)
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
        Entity[] allEntities = Resources.LoadAll<Entity>("Entity");
        Debug.Log("EntityInfo assets found: " + allEntities.Length);

        foreach (Entity e in allEntities)
        {
            Debug.Log(" - " + e.name);

            if (idLookup.ContainsKey(e.id))
            {
                throw new System.Exception("Error: Multiple scriptable objects registered to the same ID: " + e.id.ToString());
            }
            idLookup[e.id] = e;
        }

        Debug.Log("EntityInfo assets loaded into idLookup: " + idLookup.Count);
    }
}

public enum ID : short
{
    Empty,
    Ground,
    Water,
    Apple
}