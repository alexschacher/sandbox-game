using UnityEngine;

[CreateAssetMenu]
public class BlockBitmaskInfo : ScriptableObject
{
    public Mesh isleMesh;
    public Mesh endMesh;
    public Mesh laneMesh;
    public Mesh cornerMesh;
    public Mesh edgeMesh;
    public Mesh midMesh;

    public int rotateEnd;
    public int rotateLane;
    public int rotateCorner;
    public int rotateEdge;
}