using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonBillboardAnim : MonoBehaviour
{
    [SerializeField] private float animSpeed = 1f;
    [SerializeField] private int numOfFrames;
    [SerializeField] private int framePixelWidth;

    private int currentFrame;
    private float animTimer;
    private MeshRenderer rend;
    private MaterialPropertyBlock properties;

    private void Awake()
    {
        properties = new MaterialPropertyBlock();
        rend = GetComponent<MeshRenderer>();
    }
    public void SetVariables(float animSpeed, int numOfFrames, int framePixelWidth)
    {
        this.animSpeed = animSpeed;
        this.numOfFrames = numOfFrames;
        this.framePixelWidth = framePixelWidth;
    }
    private void Update()
    {
        Animate();
    }
    private void Animate()
    {
        animTimer += (animSpeed * Time.deltaTime);

        if (animTimer >= 1f)
        {
            animTimer = animTimer % 1;
            currentFrame++;

            if (currentFrame >= numOfFrames)
            {
                currentFrame = 0;
            }

            UpdateMaterialProperties(new Vector2(framePixelWidth * currentFrame, 0f));
        }
    }
    private void UpdateMaterialProperties(Vector2 offset)
    {
        rend.GetPropertyBlock(properties);
        properties.SetVector("_NonBillboardPixelOffset", offset);
        rend.SetPropertyBlock(properties);
    }
}
