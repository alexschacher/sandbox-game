using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerNetInit : NetworkBehaviour
{
    [SerializeField] private Billboard characterBillboard;
    [SerializeField] private List<Material> characterMaterials;
    [SyncVar(hook = "SetCharacterMaterial")] private int characterMaterialIndex;

    override public void OnStartAuthority()
    {
        CameraFollowTarget.SetTargetObject(transform);
        CmdSetCharacterMaterial(Random.Range(0, characterMaterials.Count));
    }

    [Command]
    public void CmdSetCharacterMaterial(int index)
    {
        characterMaterialIndex = index;
    }

    public void SetCharacterMaterial(int oldIndex, int newIndex)
    {
        //characterBillboard.SetMaterial(characterMaterials[newIndex]);
    }
}