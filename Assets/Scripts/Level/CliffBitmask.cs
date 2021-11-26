using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliffBitmask : MonoBehaviour
{
    private MeshFilter meshFilter1, meshFilter2, meshFilter3, meshFilter4;
    [SerializeField] private GameObject meshObject1, meshObject2, meshObject3, meshObject4;
    [SerializeField] private Mesh edgeMesh, cornerMesh, waterfallCornerMesh;
    private bool mesh1IsCorner, mesh2IsCorner, mesh3IsCorner, mesh4IsCorner;
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
        CheckForWaterfall();
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

                        SetMesh1ToEdge();
                        SetMesh2ToEdge();
                        SetMesh3ToEdge();
                        SetMesh4ToEdge();

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

                        SetMesh1ToEdge();
                        SetMesh2ToEdge();
                        SetMesh3ToEdge();

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

                        SetMesh1ToEdge();
                        SetMesh2ToEdge();
                        SetMesh3ToEdge();

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

                        SetMesh1ToEdge();
                        SetMesh2ToEdge();

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

                        SetMesh1ToEdge();
                        SetMesh2ToEdge();
                        SetMesh3ToEdge();

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

                            SetMesh1ToEdge();
                            SetMesh2ToEdge();
                            SetMesh3ToCorner();

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

                            SetMesh1ToEdge();
                            SetMesh2ToEdge();

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

                            SetMesh1ToEdge();
                            SetMesh2ToEdge();
                            SetMesh3ToCorner();

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

                            SetMesh1ToEdge();
                            SetMesh2ToEdge();

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

                                SetMesh1ToEdge();
                                SetMesh2ToCorner();
                                SetMesh3ToCorner();

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

                                SetMesh1ToEdge();
                                SetMesh2ToCorner();

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

                                SetMesh1ToEdge();
                                SetMesh2ToCorner();

                                meshObject1.transform.rotation = Quaternion.Euler(0, 90, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                            }
                            else
                            {
                                // #8 Edge 1/4
                                meshObject2.SetActive(false);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                SetMesh1ToEdge();

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

                        SetMesh1ToEdge();
                        SetMesh2ToEdge();
                        SetMesh3ToEdge();

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

                            SetMesh1ToEdge();
                            SetMesh2ToEdge();
                            SetMesh3ToCorner();

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

                            SetMesh1ToEdge();
                            SetMesh2ToEdge();

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

                            SetMesh1ToEdge();
                            SetMesh2ToEdge();
                            SetMesh3ToCorner();

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

                            SetMesh1ToEdge();
                            SetMesh2ToEdge();

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

                                SetMesh1ToEdge();
                                SetMesh2ToCorner();
                                SetMesh3ToCorner();

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

                                SetMesh1ToEdge();
                                SetMesh2ToCorner();

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

                                SetMesh1ToEdge();
                                SetMesh2ToCorner();

                                meshObject1.transform.rotation = Quaternion.Euler(0, -90, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, 180, 0);
                            }
                            else
                            {
                                // #12 Edge 2/4
                                meshObject2.SetActive(false);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                SetMesh1ToEdge();

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

                        SetMesh1ToEdge();
                        SetMesh2ToEdge();

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

                                SetMesh1ToEdge();
                                SetMesh2ToCorner();
                                SetMesh3ToCorner();

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

                                SetMesh1ToEdge();
                                SetMesh2ToCorner();

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

                                SetMesh1ToEdge();
                                SetMesh2ToCorner();

                                meshObject1.transform.rotation = Quaternion.Euler(0, 180, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, 0, 0);
                            }
                            else
                            {
                                // #14 Edge 3/4
                                meshObject2.SetActive(false);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                SetMesh1ToEdge();

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

                                SetMesh1ToEdge();
                                SetMesh2ToCorner();
                                SetMesh3ToCorner();

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

                                SetMesh1ToEdge();
                                SetMesh2ToCorner();

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

                                SetMesh1ToEdge();
                                SetMesh2ToCorner();

                                meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                                meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                            }
                            else
                            {
                                // #15 Edge 4/4
                                meshObject2.SetActive(false);
                                meshObject3.SetActive(false);
                                meshObject4.SetActive(false);

                                SetMesh1ToEdge();

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

                                        SetMesh1ToCorner();
                                        SetMesh2ToCorner();
                                        SetMesh3ToCorner();
                                        SetMesh4ToCorner();

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

                                        SetMesh1ToCorner();
                                        SetMesh2ToCorner();
                                        SetMesh3ToCorner();

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

                                        SetMesh1ToCorner();
                                        SetMesh2ToCorner();
                                        SetMesh3ToCorner();

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

                                        SetMesh1ToCorner();
                                        SetMesh2ToCorner();

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

                                        SetMesh1ToCorner();
                                        SetMesh2ToCorner();
                                        SetMesh3ToCorner();

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

                                        SetMesh1ToCorner();
                                        SetMesh2ToCorner();

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

                                        SetMesh1ToCorner();
                                        SetMesh2ToCorner();

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 90, 0);
                                        meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                                    }
                                    else
                                    {
                                        // #39 Corner 1/4
                                        meshObject2.SetActive(false);
                                        meshObject3.SetActive(false);
                                        meshObject4.SetActive(false);

                                        SetMesh1ToCorner();

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

                                        SetMesh1ToCorner();
                                        SetMesh2ToCorner();
                                        SetMesh3ToCorner();

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

                                        SetMesh1ToCorner();
                                        SetMesh2ToCorner();

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

                                        SetMesh1ToCorner();
                                        SetMesh2ToCorner();

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 180, 0);
                                        meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                                    }
                                    else
                                    {
                                        // #43 Corner 2/4
                                        meshObject2.SetActive(false);
                                        meshObject3.SetActive(false);
                                        meshObject4.SetActive(false);

                                        SetMesh1ToCorner();

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

                                        SetMesh1ToCorner();
                                        mesh1IsCorner = true;
                                        SetMesh2ToCorner();

                                        meshObject1.transform.rotation = Quaternion.Euler(0, 0, 0);
                                        meshObject2.transform.rotation = Quaternion.Euler(0, -90, 0);
                                    }
                                    else
                                    {
                                        // #45 Corner 3/4
                                        meshObject2.SetActive(false);
                                        meshObject3.SetActive(false);
                                        meshObject4.SetActive(false);

                                        SetMesh1ToCorner();

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

                                        SetMesh1ToCorner();

                                        meshObject1.transform.rotation = Quaternion.Euler(0, -90, 0);
                                    }
                                    else
                                    {
                                        // No neighbors found on any 4 edge or any 4 corner
                                        // This cliff should not exist! Destroy self.

                                        // TODO Should be restricted to only Host doing this..
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

    private void SetMesh1ToCorner()
    {
        if (mesh1IsCorner) return;
        meshFilter1.mesh = cornerMesh;
        mesh1IsCorner = true;
    }
    private void SetMesh2ToCorner()
    {
        if (mesh2IsCorner) return;
        meshFilter2.mesh = cornerMesh;
        mesh2IsCorner = true;
    }
    private void SetMesh3ToCorner()
    {
        if (mesh3IsCorner) return;
        meshFilter3.mesh = cornerMesh;
        mesh3IsCorner = true;
    }
    private void SetMesh4ToCorner()
    {
        if (mesh4IsCorner) return;
        meshFilter4.mesh = cornerMesh;
        mesh4IsCorner = true;
    }
    private void SetMesh1ToEdge()
    {
        mesh1IsCorner = false;
        meshFilter1.mesh = edgeMesh;
    }
    private void SetMesh2ToEdge()
    {
        mesh2IsCorner = false;
        meshFilter2.mesh = edgeMesh;
    }
    private void SetMesh3ToEdge()
    {
        mesh3IsCorner = false;
        meshFilter3.mesh = edgeMesh; ;
    }
    private void SetMesh4ToEdge()
    {
        mesh4IsCorner = false;
        meshFilter4.mesh = edgeMesh;
    }

    private void CheckForWaterfall()
    {
        GameObject waterObject = LevelHandler.GetObjectAtPosition(pos.x, pos.y + 1, pos.z);
        if (waterObject != null)
        {
            WaterBitmask waterBitmask = waterObject.GetComponent<WaterBitmask>();
            if (waterBitmask != null)
            {
                if (waterBitmask.CheckIfWaterfall() == true)
                {
                    ChangeCornersForWaterfall();
                }
            }
        }
    }

    public void ChangeCornersForWaterfall()
    {
        if (mesh1IsCorner)                              meshFilter1.mesh = waterfallCornerMesh;
        if (meshObject2.activeSelf && mesh2IsCorner)    meshFilter2.mesh = waterfallCornerMesh;
        if (meshObject3.activeSelf && mesh3IsCorner)    meshFilter3.mesh = waterfallCornerMesh;
        if (meshObject4.activeSelf && mesh4IsCorner)    meshFilter4.mesh = waterfallCornerMesh;
    }

    public void ChangeCornersForNoWaterfall()
    {
        if (mesh1IsCorner)                              meshFilter1.mesh = cornerMesh;
        if (meshObject2.activeSelf && mesh2IsCorner)    meshFilter2.mesh = cornerMesh;
        if (meshObject3.activeSelf && mesh3IsCorner)    meshFilter3.mesh = cornerMesh;
        if (meshObject4.activeSelf && mesh4IsCorner)    meshFilter4.mesh = cornerMesh;
    }
}