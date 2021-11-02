using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public static Level Generate(int levelWidth)
    {
        Level level = new Level(levelWidth);

        for (int chunkX = 0; chunkX < levelWidth; chunkX++)
        {
            for (int chunkZ = 0; chunkZ < levelWidth; chunkZ++)
            {
                for (int x = 0; x < Chunk.width; x++)
                {
                    for (int z = 0; z < Chunk.width; z++)
                    {
                        level.chunks[chunkX, 0, chunkZ].voxelIDs[x, 0, z] = ID.Ground;

                        if (Random.Range(0f, 100f) < 5f)
                        {
                            level.chunks[chunkX, 0, chunkZ].voxelIDs[x, 1, z] = ID.Tree;
                        }
                    }
                }
            }
        }
        return level;
    }
}