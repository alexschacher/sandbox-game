using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteract : NetworkBehaviour
{
    private CharCore core;
    private Fishing fishing;

    private void Awake()
    {
        fishing = GetComponent<Fishing>();
        core = GetComponent<CharCore>();
        if (core != null)
        {
            core.onInteractEvent += InteractEvent;
        }
    }

    private void InteractEvent()
    {
        if (hasAuthority == false) return;

        if (core.GetFishingState() != Fishing.State.NotFishing)
        {
            fishing.Interact(); return;
        }

        Vector3Int coords = VectorMath.GetWorldVoxelCoordsFromPosition(transform.position);
        Vector3Int dir = VectorMath.GetRelativeAdjacentVoxelFromDir(core.GetInteractDir());
        Vector3Int interactCoords = new Vector3Int(dir.x + coords.x, coords.y, dir.z + coords.z);
        Vector3Int interactCoordsBelow = new Vector3Int(interactCoords.x, interactCoords.y - 1, interactCoords.z);

        vID interactVoxel =         LevelHandler.GetVoxelIdAtPosition(interactCoords.x, interactCoords.y, interactCoords.z);
        vID interactVoxelBelow =    LevelHandler.GetVoxelIdAtPosition(interactCoords.x, interactCoords.y - 1, interactCoords.z);
        vID interactVoxelBelow2 =   LevelHandler.GetVoxelIdAtPosition(interactCoords.x, interactCoords.y - 2, interactCoords.z);

        if (interactVoxelBelow == vID.Empty && interactVoxelBelow2 == vID.Water && core.GetHeldItem() == eID.Null)
        {
            fishing.StartFishing(interactCoordsBelow);
        }
    }
}