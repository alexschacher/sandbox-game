using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Level : NetworkBehaviour
{
    private short[] testChunk;
    [SerializeField] private int chunkWidth = 8;
    [SerializeField] private int chunkHeight = 4;

    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject waterPrefab;

    private void Start()
    {
        testChunk = GenerateChunk();
    }

    private short[] GenerateChunk()
    {
        short[] chunk = new short[chunkWidth * chunkWidth * chunkHeight];

        for (int z = 0; z < chunkWidth; z++)
        {
            for (int x = 0; x < chunkWidth; x++)
            {
                chunk[Get1D(x, 0, z)] = (short)Random.Range(1, 3);

                if (Random.Range(0f, 100f) < 10f)
                {
                    chunk[Get1D(x, 1, z)] = (short)ID.Apple;
                }
            }
        }
        return chunk;
    }

    private void OnGrab()
    {
        if (isServer)
        {
            RpcSendChunk(testChunk);
        }
    }

    private void OnZoomCameraIn()
    {
        CmdModifyCell(ID.Apple, Random.Range(0, chunkWidth), 1, Random.Range(0, chunkWidth));
    }

    [ClientRpc] private void RpcSendChunk(short[] chunk)
    {
        //DebugChunk(chunk);
        SpawnChunk(chunk);
    }

    private void DebugChunk(short[] chunk)
    {
        string message = "";

        Debug.Log("--- Chunk ---");

        for (int y = 0; y < chunkHeight; y++)
        {
            Debug.Log("y: " + y);
            for (int z = 0; z < chunkWidth; z++)
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    message = message + chunk[Get1D(x, y, z)];
                }
                Debug.Log(message);
                message = "";
            }
        }
    }

    private void SpawnChunk(short[] chunk)
    {
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int z = 0; z < chunkWidth; z++)
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    SpawnObject((ID)chunk[Get1D(x, y, z)], x, y, z);
                }
            }
        }
    }

    private void SpawnObject(ID id, int x, int y, int z)
    {
        if (id == ID.Empty) return;

        Entity entity = Entity.GetFromID(id);

        GameObject spawnedObj = Instantiate(entity.prefab, new Vector3(x, y, z), Quaternion.identity, transform);

        spawnedObj.GetComponent<Spawnable>().Init(entity.values);
    }

    [Command(ignoreAuthority = true)] private void CmdModifyCell(ID id, int x, int y, int z)
    {
        testChunk[Get1D(x, y, z)] = (short)id;

        RpcModifyCell(id, x, y, z);
    }
    [ClientRpc] private void RpcModifyCell(ID id, int x, int y, int z)
    {
        SpawnObject(id, x, y, z);
    }

    private int Get1D(int x, int y, int z) => x + (z * chunkWidth) + (y * chunkWidth * chunkWidth);
}