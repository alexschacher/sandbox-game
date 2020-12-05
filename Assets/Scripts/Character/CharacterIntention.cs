using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIntention : MonoBehaviour
{
    [SerializeField] private Vector2 moveDir;
    [SerializeField] private Vector2 faceDir;

    public void SetMoveDir(Vector2 dir) => moveDir = dir;
    public void SetFaceDir(Vector2 dir) => faceDir = dir;
    
    public Vector2 GetMoveDir() => moveDir;
    public Vector2 GetFaceDir() => faceDir;
}