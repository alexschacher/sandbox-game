using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenUtil
{
    public static float[,] Normalize(float[,] inputArray, float minValue, float maxValue)
    {
        float[,] resultArray = new float[inputArray.GetLength(0), inputArray.GetLength(1)];
        float range = maxValue - minValue;

        for (int x = 0; x < inputArray.GetLength(0); x++)
        {
            for (int y = 0; y < inputArray.GetLength(1); y++)
            {
                resultArray[x, y] = (inputArray[x, y] - minValue) / range;
            }
        }
        return resultArray;
    }

    public static float[,] Invert(float[,] inputArray)
    {
        float[,] resultArray = new float[inputArray.GetLength(0), inputArray.GetLength(1)];

        for (int x = 0; x < inputArray.GetLength(0); x++)
        {
            for (int y = 0; y < inputArray.GetLength(1); y++)
            {
                resultArray[x, y] = Mathf.Abs(inputArray[x, y] - 1);
            }
        }
        return resultArray;
    }

    public static float[,] Darken(float[,] inputArray, float[,] inputArray2)
    {
        int width = Mathf.Min(inputArray.GetLength(0), inputArray2.GetLength(0));
        int height = Mathf.Min(inputArray.GetLength(1), inputArray2.GetLength(1));
        float[,] resultArray = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                resultArray[x,y] = Mathf.Min(inputArray[x, y], inputArray2[x, y]);
            }
        }
        return resultArray;
    }

    public static float[,] Lighten(float[,] inputArray, float[,] inputArray2)
    {
        int width = Mathf.Min(inputArray.GetLength(0), inputArray2.GetLength(0));
        int height = Mathf.Min(inputArray.GetLength(1), inputArray2.GetLength(1));
        float[,] resultArray = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                resultArray[x, y] = Mathf.Max(inputArray[x, y], inputArray2[x, y]);
            }
        }
        return resultArray;
    }

    public static float[,] Subtract(float[,] inputArray, float[,] subtractArray)
    {
        int width = Mathf.Min(inputArray.GetLength(0), subtractArray.GetLength(0));
        int height = Mathf.Min(inputArray.GetLength(1), subtractArray.GetLength(1));
        float[,] resultArray = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                resultArray[x, y] = Mathf.Clamp01(inputArray[x, y] -= subtractArray[x, y]);
            }
        }
        return resultArray;
    }

    public static float[,] Posterize(float[,] inputArray, float cutoff)
    {
        float[,] resultArray = new float[inputArray.GetLength(0), inputArray.GetLength(1)];

        for (int x = 0; x < inputArray.GetLength(0); x++)
        {
            for (int y = 0; y < inputArray.GetLength(1); y++)
            {
                resultArray[x, y] = inputArray[x, y] > cutoff ? 1 : 0;
            }
        }
        return resultArray;
    }

    public static Texture2D GenerateTexture(float[,] inputArray)
    {
        int width = inputArray.GetLength(0);
        int height = inputArray.GetLength(1);

        Color[] colors = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colors[(y * width) + x] = Color.Lerp(Color.black, Color.white, inputArray[x, y]);
            }
        }

        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }
     
    public static float[,] GenerateRadial(int arrayWidth, float sharpness, float fill)
    {
        float[,] radial = new float[arrayWidth, arrayWidth];
        Vector2 center = new Vector2(arrayWidth / 2, arrayWidth / 2);
        float maxDistance = Vector2.Distance(new Vector2(arrayWidth / 2, 0), center);

        for (int x = 0; x < arrayWidth; x++)
        {
            for (int y = 0; y < arrayWidth; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);

                if (distance > maxDistance)
                {
                    distance = maxDistance;
                }
                radial[x, y] = distance;
            }
        }
        radial = Normalize(radial, 0, maxDistance);

        for (int x = 0; x < arrayWidth; x++)
        {
            for (int y = 0; y < arrayWidth; y++)
            {
                radial[x, y] = Mathf.Pow(radial[x, y], sharpness) / (Mathf.Pow(radial[x, y], sharpness) + Mathf.Pow(fill - fill * radial[x, y], sharpness));
            }
        }
        return radial;
    }

    public static float[,] GenerateBlob(int arrayWidth, float posterizeCutoff, float radialSharpness, float radialFill, int perlinSeed, float perlinScale, int perlinOctaves, float perlinPersistance, float perlinLacunarity)
    {
        float[,] perlin = GeneratePerlin(arrayWidth, perlinSeed, perlinScale, perlinOctaves, perlinLacunarity, perlinPersistance);
        float[,] radial = GenerateRadial(arrayWidth, radialSharpness, radialFill);
        float[,] blob = Subtract(perlin, radial);
        blob = Posterize(blob, posterizeCutoff);
        return blob;
    }

    public static float[,] GeneratePerlin(int arrayWidth, int seed, float scale, int octaves, float persistance, float lacunarity)
    {
        float[,] perlin = new float[arrayWidth, arrayWidth];
        float maxValue = float.MinValue;
        float minValue = float.MaxValue;

        if (scale < 2f)         scale = 2f;
        if (persistance > 1f)   persistance = 1f;
        if (lacunarity < 1f)    lacunarity = 1f;

        System.Random random = new System.Random(seed);
        Vector2[] octavesOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            octavesOffsets[i] = new Vector2(random.Next(-100000, 100000), random.Next(-100000, 100000));
        }

        for (int x = 0; x < arrayWidth; x++)
        {
            for (int y = 0; y < arrayWidth; y++)
            {
                float amplitude = 1;
                float frequency = 1;
                float sumOfOctaves = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float perlinX = x / scale * frequency + octavesOffsets[i].x;
                    float perlinY = y / scale * frequency + octavesOffsets[i].y;
                    float perlinValue = Mathf.PerlinNoise(perlinX, perlinY) * 2 - 1;
                    sumOfOctaves += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                perlin[x, y] = sumOfOctaves;
                if (sumOfOctaves > maxValue)   maxValue = sumOfOctaves;
                if (sumOfOctaves < minValue)   minValue = sumOfOctaves;
            }
        }
        perlin = Normalize(perlin, minValue, maxValue);
        return perlin;
    }

    public static float[,] GeneratePoisson(int arrayWidth, float minDistance, float maxDistance)
    {
        float[,] poisson = new float[arrayWidth, arrayWidth];
        Vector2Int centerPoint = new Vector2Int(arrayWidth / 2, arrayWidth / 2);
        List<Vector2Int> activeCells = new List<Vector2Int>();
        List<Vector2Int> relativeCellsToBlock = new List<Vector2Int>();
        List<Vector2Int> relativeCellsToCheck = new List<Vector2Int>();

        int checkDist = Mathf.CeilToInt(maxDistance);
        for (int x = -checkDist; x <= checkDist; x++)
        {
            for (int y = -checkDist; y <= checkDist; y++)
            {
                Vector2Int offset = new Vector2Int(x, y);
                float distance = Vector2Int.Distance(Vector2Int.zero, offset);

                if (offset.x == 0 && offset.y == 0)
                {
                    continue;
                }
                else if (distance < minDistance)
                {
                    relativeCellsToBlock.Add(offset);
                }
                else if (distance < maxDistance)
                {
                    relativeCellsToCheck.Add(offset);
                }
            }
        }

        activeCells.Add(centerPoint);
        poisson[centerPoint.x, centerPoint.y] = 1;
        foreach (Vector2Int cellToBlock in relativeCellsToBlock)
        {
            poisson[centerPoint.x + cellToBlock.x, centerPoint.y + cellToBlock.y] = 0.5f;
        }

        while (activeCells.Count > 0)
        {
            Vector2Int randomActiveCell = activeCells[Random.Range(0, activeCells.Count)];
            bool eligibleNeighborFound = false;

            for (int i = 0; i < relativeCellsToCheck.Count; i++) // Shuffle
            {
                Vector2Int temp = relativeCellsToCheck[i];
                int randomIndex = Random.Range(i, relativeCellsToCheck.Count);
                relativeCellsToCheck[i] = relativeCellsToCheck[randomIndex];
                relativeCellsToCheck[randomIndex] = temp;
            }

            for (int i = 0; i < relativeCellsToCheck.Count; i++)
            {
                int x = randomActiveCell.x + relativeCellsToCheck[i].x;
                int y = randomActiveCell.y + relativeCellsToCheck[i].y;
                if (x < 0 || y < 0 || x > arrayWidth - 1 || y > arrayWidth - 1) continue;

                if (poisson[x, y] == 0)
                {
                    eligibleNeighborFound = true;
                    activeCells.Add(new Vector2Int(x, y));
                    poisson[x, y] = 1;
                    foreach (Vector2Int cellToBlock in relativeCellsToBlock)
                    {
                        int blockX = x + cellToBlock.x;
                        int blockY = y + cellToBlock.y;
                        if (blockX < 0 || blockY < 0 || blockX > arrayWidth - 1 || blockY > arrayWidth - 1) continue;
                        poisson[blockX, blockY] = 0.5f;
                    }
                    break;
                }
            }
            if (eligibleNeighborFound == false)
            {
                activeCells.Remove(randomActiveCell);
            }
        }
        poisson = Posterize(poisson, 0.9f);
        return poisson;
    }

    public static float[,] GenerateGrid(int arrayWidth, int xSpace, int zSpace, int rowOffset)
    {
        float[,] grid = new float[arrayWidth, arrayWidth];
        int currentRowOffset = 0;

        for (int z = 0; z < arrayWidth; z++)
        {
            if (z % zSpace != 0) continue;

            currentRowOffset = currentRowOffset == 0 ? rowOffset : 0;

            for (int x = 0; x < arrayWidth; x++)
            {
                if (x % xSpace != 0) continue;

                if (x + currentRowOffset < arrayWidth)
                {
                    grid[x + currentRowOffset, z] = 1;
                }
            }
        }
        return grid;
    }
}