using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliffBitmask : MonoBehaviour
{
    private MeshFilter meshFilter1, meshFilter2, meshFilter3, meshFilter4;
    [SerializeField] private GameObject meshObject1, meshObject2, meshObject3, meshObject4;
    [SerializeField] private Mesh edgeMesh, cornerMesh;
    private Vector3Int pos;

    private void Awake()
    {
        meshFilter1 = meshObject1.GetComponent<MeshFilter>();
        meshFilter2 = meshObject2.GetComponent<MeshFilter>();
        meshFilter3 = meshObject3.GetComponent<MeshFilter>();
        meshFilter4 = meshObject4.GetComponent<MeshFilter>();
        pos = new Vector3Int((int)transform.position.x, (int)(transform.position.y * 2), (int)transform.position.z);
    }

    private void Start()
    {
        UpdateModel();
        UpdateNeighbors();
    }

    public void UpdateModel()
    {
        bool north = LevelHandler.GetVoxelIdAtPosition(pos.x, pos.y + 2, pos.z + 1) == vID.Ground;
        bool south = LevelHandler.GetVoxelIdAtPosition(pos.x, pos.y + 2, pos.z - 1) == vID.Ground;
        bool east = LevelHandler.GetVoxelIdAtPosition(pos.x + 1, pos.y + 2, pos.z) == vID.Ground;
        bool west = LevelHandler.GetVoxelIdAtPosition(pos.x - 1, pos.y + 2, pos.z) == vID.Ground;

        bool northeast = LevelHandler.GetVoxelIdAtPosition(pos.x + 1, pos.y + 2, pos.z + 1) == vID.Ground;
        bool southeast = LevelHandler.GetVoxelIdAtPosition(pos.x + 1, pos.y + 2, pos.z - 1) == vID.Ground;
        bool northwest = LevelHandler.GetVoxelIdAtPosition(pos.x - 1, pos.y + 2, pos.z + 1) == vID.Ground;
        bool southwest = LevelHandler.GetVoxelIdAtPosition(pos.x - 1, pos.y + 2, pos.z - 1) == vID.Ground;

        if (north == true)
        {
            if (south == true)
            {
                if (east == true)
                {
                    if (west == true)
                    {
                        // #1 Trench Hole 1/1
                        meshObject2.SetActive(true);
                        meshObject3.SetActive(true);
                        meshObject4.SetActive(true);

                        meshFilter1.mesh = edgeMesh;
                        meshFilter2.mesh = edgeMesh;
                        meshFilter3.mesh = edgeMesh;
                        meshFilter4.mesh = edgeMesh;

                        meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                        meshObject2.transform.rotation = Quaternion.Euler(0, 90, 0);
                        meshObject3.transform.rotation = Quaternion.Euler(0, 180, 0);
                        meshObject4.transform.rotation = Quaternion.Euler(0, -90, 0);
                    }
                    else
                    {
                        // #2 Trench End 1/4
                        meshObject2.SetActive(true);
                        meshObject3.SetActive(true);
                        meshObject4.SetActive(false);

                        meshFilter1.mesh = edgeMesh;
                        meshFilter2.mesh = edgeMesh;
                        meshFilter3.mesh = edgeMesh;

                        meshObject1.transform.rotation = Quaternion.Euler(0, 90, 0);
                        meshObject2.transform.rotation = Quaternion.Euler(0, 180, 0);
                        meshObject3.transform.rotation = Quaternion.Euler(0, -90, 0);
                    }
                }
                else // north & south true, east false
                {
                    if (west == true)
                    {
                        // #3 Trench End 2/4
                        meshObject2.SetActive(true);
                        meshObject3.SetActive(true);
                        meshObject4.SetActive(false);

                        meshFilter1.mesh = edgeMesh;
                        meshFilter2.mesh = edgeMesh;
                        meshFilter3.mesh = edgeMesh;

                        meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                        meshObject2.transform.rotation = Quaternion.Euler(0, 90, 0);
                        meshObject3.transform.rotation = Quaternion.Euler(0, -90, 0);
                    }
                    else
                    {
                        // #4 Trench Straight 1/2
                        meshObject2.SetActive(true);
                        meshObject3.SetActive(false);
                        meshObject4.SetActive(false);

                        meshFilter1.mesh = edgeMesh;
                        meshFilter2.mesh = edgeMesh;

                        meshObject1.transform.rotation = Quaternion.Euler(0, 90, 0);
                        meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                    }
                }
            }
            else // north true, south false
            {
                if (east == true)
                {
                    if (west == true)
                    {
                        // #5 Trench End 3/4
                        meshObject2.SetActive(true);
                        meshObject3.SetActive(true);
                        meshObject4.SetActive(false);

                        meshFilter1.mesh = edgeMesh;
                        meshFilter2.mesh = edgeMesh;
                        meshFilter3.mesh = edgeMesh;

                        meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                        meshObject2.transform.rotation = Quaternion.Euler(0, 90, 0);
                        meshObject3.transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else
                    {
                        if (southwest == true)
                        {
                            // #16 Trench Corner 1/4
                            meshObject2.SetActive(true);
                            meshObject3.SetActive(true);
                            meshObject4.SetActive(false);

                            meshFilter1.mesh = edgeMesh;
                            meshFilter2.mesh = edgeMesh;
                            meshFilter3.mesh = cornerMesh;

                            meshObject1.transform.rotation = Quaternion.Euler(0, 90, 0);
                            meshObject2.transform.rotation = Quaternion.Euler(0, 180, 0);
                            meshObject3.transform.rotation = Quaternion.Euler(0, 0, 0);
                        }
                        else
                        {
                            // #6 Inner Corner 1/4
                            meshObject2.SetActive(true);
                            meshObject3.SetActive(false);
                            meshObject4.SetActive(false);

                            meshFilter1.mesh = edgeMesh;
                            meshFilter2.mesh = edgeMesh;

                            meshObject1.transform.rotation = Quaternion.Euler(0, 90, 0);
                            meshObject2.transform.rotation = Quaternion.Euler(0, 180, 0);
                        }
                    }
                }
                else // north true, south & east false
                {
                    if (west == true)
                    {
                        if (southeast == true)
                        {
                            // #17 Trench Corner 2/4
                            meshObject2.SetActive(true);
                            meshObject3.SetActive(true);
                            meshObject4.SetActive(false);

                            meshFilter1.mesh = edgeMesh;
                            meshFilter2.mesh = edgeMesh;
                            meshFilter3.mesh = cornerMesh;

                            meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                            meshObject2.transform.rotation = Quaternion.Euler(0, 90, 0);
                            meshObject3.transform.rotation = Quaternion.Euler(0, -90, 0);
                        }
                        else
                        {
                            // #7 Inner Corner 2/4
                            meshObject2.SetActive(true);
                            meshObject3.SetActive(false);
                            meshObject4.SetActive(false);

                            meshFilter1.mesh = edgeMesh;
                            meshFilter2.mesh = edgeMesh;

                            meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                            meshObject2.transform.rotation = Quaternion.Euler(0, 90, 0);
                        }
                    }
                    else
                    {
                        if (southwest == true)
                        {
                            if (southeast == true)
                            {
                                // #20 Trench T 1/4
                                meshObject2.SetActive(true);
                                meshObject3.SetActive(true);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;
                                meshFilter2.mesh = cornerMesh;
                                meshFilter3.mesh = cornerMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, 90, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                                meshObject3.transform.rotation = Quaternion.Euler(0, 0, 0);
                            }
                            else
                            {
                                // #21 Edge w Corner Right 1/4
                                meshObject2.SetActive(true);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;
                                meshFilter2.mesh = cornerMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, 90, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, 0, 0);
                            }
                        }
                        else
                        {
                            if (southeast == true)
                            {
                                // #22 Edge w Corner Left 1/4
                                meshObject2.SetActive(true);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;
                                meshFilter2.mesh = cornerMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, 90, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                            }
                            else
                            {
                                // #8 Edge 1/4
                                meshObject2.SetActive(false);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, 90, 0);
                            }
                        }
                    }
                }
            }
        }
        else // north false
        {
            if (south == true)
            {
                if (east == true)
                {
                    if (west == true)
                    {
                        // #9 Trench End 4/4
                        meshObject2.SetActive(true);
                        meshObject3.SetActive(true);
                        meshObject4.SetActive(false);

                        meshFilter1.mesh = edgeMesh;
                        meshFilter2.mesh = edgeMesh;
                        meshFilter3.mesh = edgeMesh;

                        meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                        meshObject2.transform.rotation = Quaternion.Euler(0, 180, 0);
                        meshObject3.transform.rotation = Quaternion.Euler(0, -90, 0);
                    }
                    else
                    {
                        if (northwest)
                        {
                            // #18 Trench Corner 3/4
                            meshObject2.SetActive(true);
                            meshObject3.SetActive(true);
                            meshObject4.SetActive(false);

                            meshFilter1.mesh = edgeMesh;
                            meshFilter2.mesh = edgeMesh;
                            meshFilter3.mesh = cornerMesh;

                            meshObject1.transform.rotation = Quaternion.Euler(0, 180, 0);
                            meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                            meshObject3.transform.rotation = Quaternion.Euler(0, 90, 0);
                        }
                        else
                        {
                            // #10 Inner Corner 3/4
                            meshObject2.SetActive(true);
                            meshObject3.SetActive(false);
                            meshObject4.SetActive(false);

                            meshFilter1.mesh = edgeMesh;
                            meshFilter2.mesh = edgeMesh;

                            meshObject1.transform.rotation = Quaternion.Euler(0, 180, 0);
                            meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                        }
                    }
                }
                else // south true, north & east false
                {
                    if (west == true)
                    {
                        if (northeast == true)
                        {
                            // #19 Trench Corner 4/4
                            meshObject2.SetActive(true);
                            meshObject3.SetActive(true);
                            meshObject4.SetActive(false);

                            meshFilter1.mesh = edgeMesh;
                            meshFilter2.mesh = edgeMesh;
                            meshFilter3.mesh = cornerMesh;

                            meshObject1.transform.rotation = Quaternion.Euler(0, -90, 0);
                            meshObject2.transform.rotation = Quaternion.Euler(0, 0, 0);
                            meshObject3.transform.rotation = Quaternion.Euler(0, 180, 0);
                        }
                        else
                        {
                            // #11 Inner Corner 4/4
                            meshObject2.SetActive(true);
                            meshObject3.SetActive(false);
                            meshObject4.SetActive(false);

                            meshFilter1.mesh = edgeMesh;
                            meshFilter2.mesh = edgeMesh;

                            meshObject1.transform.rotation = Quaternion.Euler(0, -90, 0);
                            meshObject2.transform.rotation = Quaternion.Euler(0, 0, 0);
                        }
                    }
                    else
                    {
                        if (northwest == true)
                        {
                            if (northeast == true)
                            {
                                // #23 Trench T 2/4
                                meshObject2.SetActive(true);
                                meshObject3.SetActive(true);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;
                                meshFilter2.mesh = cornerMesh;
                                meshFilter3.mesh = cornerMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, -90, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, 90, 0);
                                meshObject3.transform.rotation = Quaternion.Euler(0, 180, 0);
                            }
                            else
                            {
                                // #24 Edge w Corner Left 2/4
                                meshObject2.SetActive(true);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;
                                meshFilter2.mesh = cornerMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, -90, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, 90, 0);
                            }
                        }
                        else
                        {
                            if (northeast == true)
                            {
                                // #25 Edge w Corner Right 2/4
                                meshObject2.SetActive(true);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;
                                meshFilter2.mesh = cornerMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, -90, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, 180, 0);
                            }
                            else
                            {
                                // #12 Edge 2/4
                                meshObject2.SetActive(false);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, -90, 0);
                            }
                        }
                    }
                }
            }
            else // north & south false
            {
                if (east == true)
                {
                    if (west == true)
                    {
                        // #13 Trench Straight 2/2
                        meshObject2.SetActive(true);
                        meshObject3.SetActive(false);
                        meshObject4.SetActive(false);

                        meshFilter1.mesh = edgeMesh;
                        meshFilter2.mesh = edgeMesh;

                        meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                        meshObject2.transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else
                    {
                        if (northwest == true)
                        {
                            if (southwest == true)
                            {
                                // #26 Trench T 3/4
                                meshObject2.SetActive(true);
                                meshObject3.SetActive(true);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;
                                meshFilter2.mesh = cornerMesh;
                                meshFilter3.mesh = cornerMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, 180, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, 0, 0);
                                meshObject3.transform.rotation = Quaternion.Euler(0, 90, 0);
                            }
                            else
                            {
                                // #27 Edge w Corner Right 3/4
                                meshObject2.SetActive(true);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;
                                meshFilter2.mesh = cornerMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, 180, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, 90, 0);
                            }
                        }
                        else
                        {
                            if (southwest == true)
                            {
                                // #28 Edge w Corner Left 3/4
                                meshObject2.SetActive(true);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;
                                meshFilter2.mesh = cornerMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, 180, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, 0, 0);
                            }
                            else
                            {
                                // #14 Edge 3/4
                                meshObject2.SetActive(false);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, 180, 0);
                            }
                        }
                    }
                }
                else // north, south & east false
                {
                    if (west == true)
                    {
                        if (northeast == true)
                        {
                            if (southeast == true)
                            {
                                // #29 Trench T 4/4
                                meshObject2.SetActive(true);
                                meshObject3.SetActive(true);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;
                                meshFilter2.mesh = cornerMesh;
                                meshFilter3.mesh = cornerMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, 180, 0);
                                meshObject3.transform.rotation = Quaternion.Euler(0, -90, 0);
                            }
                            else
                            {
                                // #30 Edge w Corner Left 4/4
                                meshObject2.SetActive(true);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;
                                meshFilter2.mesh = cornerMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, 180, 0);
                            }
                        }
                        else
                        {
                            if (southeast == true)
                            {
                                // #31 Edge w Corner Right 4/4
                                meshObject2.SetActive(true);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;
                                meshFilter2.mesh = cornerMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                            }
                            else
                            {
                                // #15 Edge 4/4
                                meshObject2.SetActive(false);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                meshFilter1.mesh = edgeMesh;

                                meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                            }
                        }
                    }
                    else // north, south, east, and west edges are false
                    {
                        if (northwest)
                        {
                            if (northeast)
                            {
                                if (southwest)
                                {
                                    if (southeast)
                                    {
                                        // #32 Trench Intersection 1/1
                                        meshObject2.SetActive(true);
                                        meshObject3.SetActive(true);
                                        meshObject4.SetActive(true);

                                        meshFilter1.mesh = cornerMesh;
                                        meshFilter2.mesh = cornerMesh;
                                        meshFilter3.mesh = cornerMesh;
                                        meshFilter4.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                                        meshObject2.transform.rotation = Quaternion.Euler(0, 90, 0);
                                        meshObject3.transform.rotation = Quaternion.Euler(0, 180, 0);
                                        meshObject4.transform.rotation = Quaternion.Euler(0, -90, 0);
                                    }
                                    else
                                    {
                                        // #33 Triple Corner 1/4
                                        meshObject2.SetActive(true);
                                        meshObject3.SetActive(true);
                                        meshObject4.SetActive(false);

                                        meshFilter1.mesh = cornerMesh;
                                        meshFilter2.mesh = cornerMesh;
                                        meshFilter3.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                                        meshObject2.transform.rotation = Quaternion.Euler(0, 90, 0);
                                        meshObject3.transform.rotation = Quaternion.Euler(0, 180, 0);
                                    }
                                }
                                else
                                {
                                    if (southeast)
                                    {
                                        // #34 Triple Corner 2/4
                                        meshObject2.SetActive(true);
                                        meshObject3.SetActive(true);
                                        meshObject4.SetActive(false);

                                        meshFilter1.mesh = cornerMesh;
                                        meshFilter2.mesh = cornerMesh;
                                        meshFilter3.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 90, 0);
                                        meshObject2.transform.rotation = Quaternion.Euler(0, 180, 0);
                                        meshObject3.transform.rotation = Quaternion.Euler(0, -90, 0);
                                    }
                                    else
                                    {
                                        // #35 Double Corner Adjacent 1/4
                                        meshObject2.SetActive(true);
                                        meshObject3.SetActive(false);
                                        meshObject4.SetActive(false);

                                        meshFilter1.mesh = cornerMesh;
                                        meshFilter2.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 90, 0);
                                        meshObject2.transform.rotation = Quaternion.Euler(0, 180, 0);
                                    }
                                }
                            }
                            else // northwest true, northeast false
                            {
                                if (southwest)
                                {
                                    if (southeast)
                                    {
                                        // #36 Triple Corner 3/4
                                        meshObject2.SetActive(true);
                                        meshObject3.SetActive(true);
                                        meshObject4.SetActive(false);

                                        meshFilter1.mesh = cornerMesh;
                                        meshFilter2.mesh = cornerMesh;
                                        meshFilter3.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, -90, 0);
                                        meshObject2.transform.rotation = Quaternion.Euler(0, 0, 0);
                                        meshObject3.transform.rotation = Quaternion.Euler(0, 90, 0);
                                    }
                                    else
                                    {
                                        // #37 Double Corner Adjacent 2/4
                                        meshObject2.SetActive(true);
                                        meshObject3.SetActive(false);
                                        meshObject4.SetActive(false);

                                        meshFilter1.mesh = cornerMesh;
                                        meshFilter2.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                                        meshObject2.transform.rotation = Quaternion.Euler(0, 90, 0);
                                    }
                                }
                                else
                                {
                                    if (southeast)
                                    {
                                        // #38 Double Corner Opposite 1/2
                                        meshObject2.SetActive(true);
                                        meshObject3.SetActive(false);
                                        meshObject4.SetActive(false);

                                        meshFilter1.mesh = cornerMesh;
                                        meshFilter2.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 90, 0);
                                        meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                                    }
                                    else
                                    {
                                        // #39 Corner 1/4
                                        meshObject2.SetActive(false);
                                        meshObject3.SetActive(false);
                                        meshObject4.SetActive(false);

                                        meshFilter1.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 90, 0);
                                    }
                                }
                            }
                        }
                        else // northwest false
                        {
                            if (northeast)
                            {
                                if (southwest)
                                {
                                    if (southeast)
                                    {
                                        // #40 Triple Corner 4/4
                                        meshObject2.SetActive(true);
                                        meshObject3.SetActive(true);
                                        meshObject4.SetActive(false);

                                        meshFilter1.mesh = cornerMesh;
                                        meshFilter2.mesh = cornerMesh;
                                        meshFilter3.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 180, 0);
                                        meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                                        meshObject3.transform.rotation = Quaternion.Euler(0, 0, 0);
                                    }
                                    else
                                    {
                                        // #41 Double Corner Opposite 2/2
                                        meshObject2.SetActive(true);
                                        meshObject3.SetActive(false);
                                        meshObject4.SetActive(false);

                                        meshFilter1.mesh = cornerMesh;
                                        meshFilter2.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                                        meshObject2.transform.rotation = Quaternion.Euler(0, 180, 0);
                                    }
                                }
                                else
                                {
                                    if (southeast)
                                    {
                                        // #42 Double Corner Adjacent 3/4
                                        meshObject2.SetActive(true);
                                        meshObject3.SetActive(false);
                                        meshObject4.SetActive(false);

                                        meshFilter1.mesh = cornerMesh;
                                        meshFilter2.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 180, 0);
                                        meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                                    }
                                    else
                                    {
                                        // #43 Corner 2/4
                                        meshObject2.SetActive(false);
                                        meshObject3.SetActive(false);
                                        meshObject4.SetActive(false);

                                        meshFilter1.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 180, 0);
                                    }
                                }
                            }
                            else // northwest & northeast false
                            {
                                if (southwest)
                                {
                                    if (southeast)
                                    {
                                        // #44 Double Corner Adjacent 4/4
                                        meshObject2.SetActive(true);
                                        meshObject3.SetActive(false);
                                        meshObject4.SetActive(false);

                                        meshFilter1.mesh = cornerMesh;
                                        meshFilter2.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                                        meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                                    }
                                    else
                                    {
                                        // #45 Corner 3/4
                                        meshObject2.SetActive(false);
                                        meshObject3.SetActive(false);
                                        meshObject4.SetActive(false);

                                        meshFilter1.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                                    }
                                }
                                else
                                {
                                    if (southeast)
                                    {
                                        // #46 Corner 4/4
                                        meshObject2.SetActive(false);
                                        meshObject3.SetActive(false);
                                        meshObject4.SetActive(false);

                                        meshFilter1.mesh = cornerMesh;

                                        meshObject1.transform.rotation = Quaternion.Euler(0, -90, 0);
                                    }
                                    else
                                    {
                                        // No neighbors found on any 4 edge or any 4 corner
                                        // This cliff should not exist! Destroy self.
                                        Debug.Log("Cliff should not exist! Destroying it");
                                        LevelHandler.ModifyVoxel(vID.Empty, pos.x, pos.y, pos.z);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void UpdateNeighbors()
    {
        UpdateNeighbor(pos.x + 1, pos.y, pos.z);
        UpdateNeighbor(pos.x - 1, pos.y, pos.z);
        UpdateNeighbor(pos.x, pos.y, pos.z + 1);
        UpdateNeighbor(pos.x, pos.y, pos.z - 1);
    }

    private void UpdateNeighbor(int x, int y, int z)
    {
        GameObject neighbor = LevelHandler.GetObjectAtPosition(x, y, z);
        if (neighbor != null)
        {
            CliffBitmask cliffBitmask = neighbor.GetComponent<CliffBitmask>();
            if (cliffBitmask != null)
            {
                cliffBitmask.UpdateModel();
            }
        }
    }

    private void OnDestroy() => UpdateNeighbors();
}
