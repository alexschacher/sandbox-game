using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FaceDir : NetworkBehaviour
{
    [SyncVar] private Vector2 faceDir;

    public void SetFaceDir(Vector2 dir)
    {
        faceDir = dir;
        CmdSetFaceDir(dir);
    }

    [Command] private void CmdSetFaceDir(Vector2 dir) => faceDir = dir;

    public Vector2 GetFaceDir() => faceDir;
}