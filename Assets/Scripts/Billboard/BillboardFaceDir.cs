using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Billboard), typeof(FaceDir))]
public class BillboardFaceDir : MonoBehaviour
{
    private Billboard billboard;
    private FaceDir faceDir;
    private bool flipX = false;
    private bool prevFlipX = false;

    private void Awake()
    {
        billboard = GetComponent<Billboard>();
        faceDir = GetComponent<FaceDir>();
    }
    private void Update()
    {
        flipX = VectorMath.DetermineBillboardFlipX(faceDir.GetFaceDir(), flipX, Camera.main.transform);

        if (flipX != prevFlipX)
        {
            billboard.SetFlipX(flipX);
        }
        
        prevFlipX = flipX;
    }
}