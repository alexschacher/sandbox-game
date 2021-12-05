using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    private enum LevelGenID
    {
        Empty,
        Path,
        Blocked,
        Ground,
        Water,
        Tree,
        Tallgrass
    }

    public static Level Generate(int levelWidth)
    {
        LevelGenID[,,] levelGenIDs =  GenerateIslandWithTrees(levelWidth);
        return ConvertGeneratedDataToLevel(levelGenIDs, levelWidth);
    }

    private static LevelGenID[,,] GenerateIslandWithTrees(int levelWidthInChunks)
    {
        int seed = Random.Range(0, 1000);
        int arrayWidth = levelWidthInChunks * Chunk.width;

        float[,] island = LevelGenUtil.GenerateBlob(arrayWidth, 0.2f, 1f, 4f, seed, 50f, 4, 0.5f, 2f);
        float[,] treesPerlin = LevelGenUtil.GeneratePerlin(arrayWidth, seed, 15f, 4, 0.5f, 2f);
        float[,] treesPosterizedPerlin = LevelGenUtil.Posterize(treesPerlin, 0.55f);
        float[,] treesPoisson = LevelGenUtil.GeneratePoisson(arrayWidth, 2f, 2.85f);
        float[,] treesPoissonAndPerlin = LevelGenUtil.Darken(treesPoisson, treesPosterizedPerlin);
        float[,] treeMap = LevelGenUtil.Darken(island, treesPoissonAndPerlin);

        LevelGenID[,,] levelGenIDs = new LevelGenID[levelWidthInChunks * Chunk.width, Chunk.height, levelWidthInChunks * Chunk.width];

        for (int chunkX = 0; chunkX < levelWidthInChunks; chunkX++)
        {
            for (int chunkZ = 0; chunkZ < levelWidthInChunks; chunkZ++)
            {
                for (int x = 0; x < Chunk.width; x++)
                {
                    for (int z = 0; z < Chunk.width; z++)
                    {
                        Vector3Int worldCoords = LevelUtil.GetWorldCoords(chunkX, 0, chunkZ, x, 0, z);

                        levelGenIDs[worldCoords.x, 1, worldCoords.z] = LevelGenID.Water;

                        if (island[worldCoords.x, worldCoords.z] > 0.5f)
                        {
                            levelGenIDs[worldCoords.x, 2, worldCoords.z] = LevelGenID.Ground;

                            if (Random.Range(0f, 1f) < 0.4f)
                            {
                                //levelGenIDs[worldCoords.x, 3, worldCoords.z] = LevelGenID.Tallgrass;
                            }
                            if (Random.Range(0f, 1f) < 0.01f)
                            {
                                levelGenIDs[worldCoords.x, 3, worldCoords.z] = LevelGenID.Tree;
                            }
                            if (treeMap[worldCoords.x, worldCoords.z] > 0.5f)
                            {
                                levelGenIDs[worldCoords.x, 3, worldCoords.z] = LevelGenID.Tree;
                            }
                        }
                    }
                }
            }
        }
        return levelGenIDs;
    }

    private static LevelGenID[,,] GenerateEmptyLevel(int levelWidthInChunks)
    {
        LevelGenID[,,] levelGenIDs = new LevelGenID[levelWidthInChunks * Chunk.width, Chunk.height, levelWidthInChunks * Chunk.width];

        for (int chunkX = 0; chunkX < levelWidthInChunks; chunkX++)
        {
            for (int chunkZ = 0; chunkZ < levelWidthInChunks; chunkZ++)
            {
                for (int x = 0; x < Chunk.width; x++)
                {
                    for (int z = 0; z < Chunk.width; z++)
                    {
                        Vector3Int worldCoords = LevelUtil.GetWorldCoords(chunkX, 0, chunkZ, x, 0, z);

                        levelGenIDs[worldCoords.x, 1, worldCoords.z] = LevelGenID.Water;

                        if ((chunkX > 0) &&
                            (chunkZ > 0) &&
                            (chunkX < levelWidthInChunks - 1) &&
                            (chunkZ < levelWidthInChunks - 1))
                        {
                            levelGenIDs[worldCoords.x, 2, worldCoords.z] = LevelGenID.Ground;
                        }
                    }
                }
            }
        }
        return levelGenIDs;
    }

    private static Level ConvertGeneratedDataToLevel(LevelGenID[,,] levelGenIDs, int levelWidth)
    {
        Level level = new Level(levelWidth);

        for (int chunkX = 0; chunkX < levelWidth; chunkX++)
        {
            for (int chunkZ = 0; chunkZ < levelWidth; chunkZ++)
            {
                for (int y = 0; y < Chunk.height; y++)
                {
                    for (int x = 0; x < Chunk.width; x++)
                    {
                        for (int z = 0; z < Chunk.width; z++)
                        {
                            switch (levelGenIDs[(chunkX * Chunk.width) + x, y, (chunkZ * Chunk.width) + z])
                            {
                                case LevelGenID.Water:      level.chunks[chunkX, 0, chunkZ].voxelIDs[x, y, z] = vID.Water;      break;
                                case LevelGenID.Ground:     level.chunks[chunkX, 0, chunkZ].voxelIDs[x, y, z] = vID.Ground;     break;
                                case LevelGenID.Tree:       level.chunks[chunkX, 0, chunkZ].voxelIDs[x, y, z] = vID.Tree;       break;
                                case LevelGenID.Tallgrass:  level.chunks[chunkX, 0, chunkZ].voxelIDs[x, y, z] = vID.Tallgrass;  break;
                                default: break;
                            }
                        }
                    }
                }
            }
        }
        return level;
    }
}