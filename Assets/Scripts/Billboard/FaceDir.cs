using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDir : MonoBehaviour
{
    [SerializeField] private Vector2 faceDir;
    public void SetFaceDir(Vector2 dir) => faceDir = dir;
    public Vector2 GetFaceDir() => faceDir;
}