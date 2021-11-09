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
                        if ((chunkX == 0 && x == 0) ||
                            (chunkZ == 0 && z == 0) ||
                            (chunkX == levelWidth - 1 && x == Chunk.width - 1) ||
                            (chunkZ == levelWidth - 1 && z == Chunk.width - 1))
                        {
                            level.chunks[chunkX, 0, chunkZ].voxelIDs[x, 0, z] = vID.Water;
                        }
                        else
                        {
                            level.chunks[chunkX, 0, chunkZ].voxelIDs[x, 0, z] = vID.Ground;
                            if (Random.Range(0f, 100f) < 20f)
                            {
                                level.chunks[chunkX, 0, chunkZ].voxelIDs[x, 1, z] = vID.Tallgrass;
                            }
                            if (Random.Range(0f, 100f) < 1.5f)
                            {
                                level.chunks[chunkX, 0, chunkZ].voxelIDs[x, 1, z] = vID.Tree;
                            }
                        }
                    }
                }
            }
        }
        return level;
    }
}