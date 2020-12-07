using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Billboard))]
public class BillboardRandomize : MonoBehaviour
{
    private Billboard billboard;

    [SerializeField] private bool randomFlipX;
    [SerializeField] private bool randomFlipY;
    [SerializeField] private int originFrameXOffsetRange;

    private void Awake()
    {
        billboard = GetComponent<Billboard>();
    }

    private void Start()
    {
        if (randomFlipX)
        {
            if (Random.Range(0, 2) == 0) billboard.SetFlipX(true);
        }
        if (randomFlipY)
        {
            if (Random.Range(0, 2) == 0) billboard.SetFlipY(true);
        }
        Vector2 originFrame = billboard.GetOriginFrame();
        billboard.SetOriginFrame((int)originFrame.x + Random.Range(0, originFrameXOffsetRange + 1), (int)originFrame.y);
    }
}