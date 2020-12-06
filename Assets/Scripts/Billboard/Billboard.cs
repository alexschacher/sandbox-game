using UnityEngine;

[ExecuteAlways]
public class Billboard : MonoBehaviour
{
    private MeshRenderer rend;
    private MaterialPropertyBlock properties;

    [SerializeField] private float frameWidth;
    [SerializeField] private float frameSpacing;
    [SerializeField] private bool flipX;
    [SerializeField] private bool flipY;
    [SerializeField] private Vector2 originFrame;
    [SerializeField] private Vector2 animFrame;

    private void Awake()
    {
        properties = new MaterialPropertyBlock();
        rend = GetComponent<MeshRenderer>();
        UpdateMaterialPropertiesInit();
        UpdateMaterialProperties();
    }
    private void UpdateMaterialPropertiesInit()
    {
        rend.GetPropertyBlock(properties);
            properties.SetFloat("_FrameWidth", frameWidth);
            properties.SetFloat("_FrameSpacing", frameSpacing);
            properties.SetFloat("_IsBillboarded", 1f);
        rend.SetPropertyBlock(properties);
    }
    private void UpdateMaterialProperties()
    {
        rend.GetPropertyBlock(properties);
            properties.SetFloat("_FlipX", flipX ? 1f : 0f);
            properties.SetFloat("_FlipY", flipY ? 1f : 0f);
            properties.SetVector("_OriginFrame", originFrame);
            properties.SetVector("_AnimFrame", animFrame);
        rend.SetPropertyBlock(properties);
    }
    public void SetFlipY(bool input)
    {
        flipY = input;
        UpdateMaterialProperties();
    }
    public void SetFlipX(bool input)
    {
        flipX = input;
        UpdateMaterialProperties();
    }
    public void SetOriginFrame(int x, int y)
    {
        originFrame = new Vector2(x, y);
        UpdateMaterialProperties();
    }
    public void SetAnimFrame(int x, int y)
    {
        animFrame = new Vector2(x, y);
        UpdateMaterialProperties();
    }
    public void SetVisiblity(bool visibility)
    {
        rend.enabled = visibility;
    }
}