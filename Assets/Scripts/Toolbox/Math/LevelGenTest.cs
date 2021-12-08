using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelGenTest : MonoBehaviour
{
    [SerializeField] private Renderer textureRenderer;
    [SerializeField] private Renderer textureRenderer2;
    [SerializeField] private Renderer textureRenderer3;
    [SerializeField] private Renderer textureRenderer4;
    [SerializeField] public bool autoUpdate;
    [SerializeField] private int arrayWidth = 50;
    [SerializeField] private float radialSharpness = 3f;
    [SerializeField] private float radialFill = 2f;
    [SerializeField] private float poissonMin = 1.25f;
    [SerializeField] private float poissonMax = 2.85f;
    [Range(1, 100)] [SerializeField] private float perlinScale = 25f;
    [Range(1, 10)] [SerializeField] private int perlinOctaves = 4;
    [Range(0, 1)][SerializeField] private float perlinPersistance = 0.5f;
    [Range(1, 10)] [SerializeField] private float perlinLacunarity = 2f;
    [Range(1, 100)] [SerializeField] private float treesPerlinScale = 25f;
    [Range(0, 1)] [SerializeField] private float treesPosterizeCutoff = 0.1f;
    [SerializeField] private int perlinSeed;

    public void Generate(int seed)
    {
        if (seed == 0)
        {
            seed = perlinSeed;
        }
        else
        {
            perlinSeed = seed;
        }

        int arrayWidth = 256;
        float[,] treesPerlin = LevelGenUtil.GeneratePerlin(arrayWidth, seed, 15f, 4, 0.5f, 2f);
        float[,] treesPerlin2 = LevelGenUtil.GeneratePerlin(arrayWidth, seed, perlinScale, 4, 0.5f, 2f);
        treesPerlin2 = LevelGenUtil.Posterize(treesPerlin2, treesPosterizeCutoff);

        float[,] treesPosterizedPerlin = LevelGenUtil.Posterize(treesPerlin, 0.55f);
        float[,] treesPoisson = LevelGenUtil.GeneratePoisson(arrayWidth, 1.25f, 2.85f);
        float[,] treeMap = LevelGenUtil.Darken(treesPoisson, treesPosterizedPerlin);

        float[,] treesCombined = LevelGenUtil.Lighten(treesPosterizedPerlin, treesPerlin2);
        float[,] grid = LevelGenUtil.GenerateGrid(arrayWidth, 2, 2, 1);
        grid = LevelGenUtil.Darken(grid, treesCombined);

        float[,] island = LevelGenUtil.GenerateBlob(arrayWidth, 0.2f, 1f, 4f, seed, 50f, 4, 0.5f, 2f);
        float[,] islandHalved = LevelGenUtil.Normalize(island, 0, 2);
        float[,] islandTreeMap = LevelGenUtil.Darken(island, treeMap);
        float[,] islandHalvedWithTrees = LevelGenUtil.Lighten(islandHalved, islandTreeMap);

        textureRenderer.sharedMaterial.mainTexture = LevelGenUtil.GenerateTexture(treesPosterizedPerlin);
        textureRenderer2.sharedMaterial.mainTexture = LevelGenUtil.GenerateTexture(treesPerlin2);
        textureRenderer3.sharedMaterial.mainTexture = LevelGenUtil.GenerateTexture(treesCombined);
        textureRenderer4.sharedMaterial.mainTexture = LevelGenUtil.GenerateTexture(grid);
    }
}

[CustomEditor(typeof(LevelGenTest))]
public class LevelGenTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelGenTest levelGenTest = (LevelGenTest)target;

        if (DrawDefaultInspector())
        {
            if (levelGenTest.autoUpdate)
            {
                levelGenTest.Generate(0);
            }
        }

        if (GUILayout.Button("Generate"))
        {
            levelGenTest.Generate(Random.Range(0, 1000));
        }
    }
}
