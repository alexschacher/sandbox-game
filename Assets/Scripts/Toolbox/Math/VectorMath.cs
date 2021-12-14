using UnityEngine;

public static class VectorMath
{
    public static Vector2 ConvertInputToWorldDir(Vector2 input, Transform cam)
    {
        return ((GetCamFwd(cam) * input.y) + (GetCamRight(cam) * input.x)).normalized;
    }

    public static bool DetermineBillboardFlipX(Vector2 dir, bool currentFlip, Transform cam)
    {
        float deadZone = 0.01f;
        Vector2 camRight = GetCamRight(cam);

        if (Vector3.Dot(dir, camRight) < -deadZone) return true;
        else if (Vector3.Dot(dir, camRight) > deadZone) return false;
        return currentFlip;
    }

    public static Vector2 GetCamFwd(Transform cam) => new Vector2(cam.forward.x, cam.forward.z).normalized;
    public static Vector2 GetCamRight(Transform cam) => new Vector2(cam.right.x, cam.right.z).normalized;

    public static Vector2 V3toV2Norm(Vector3 v3)
    {  
        v3 = new Vector3(v3.x, 0f, v3.z).normalized;
        return new Vector2(v3.x, v3.z);
    }
    public static Vector2 V3toV2Raw(Vector3 v3) => new Vector2(v3.x, v3.z);
    public static Vector3 V2toV3(Vector2 v2) => new Vector3(v2.x, 0f, v2.y);

    public static Vector3 RotateAroundY(Vector3 v3, float angle)
    {
        return Quaternion.Euler(0, angle, 0) * v3;
    }

    public static Vector3Int GetRelativeAdjacentVoxelFromDir(Vector2 dir)
    {
        if (dir.x >  0.92f) return new Vector3Int( 1, 0, 0);
        if (dir.y >  0.92f) return new Vector3Int( 0, 0, 1);
        if (dir.x < -0.92f) return new Vector3Int(-1, 0, 0);
        if (dir.y < -0.92f) return new Vector3Int( 0, 0,-1);

        if (dir.x > 0 && dir.y > 0) return new Vector3Int( 1, 0, 1);
        if (dir.x < 0 && dir.y > 0) return new Vector3Int(-1, 0, 1);
        if (dir.x > 0 && dir.y < 0) return new Vector3Int( 1, 0,-1);
        if (dir.x < 0 && dir.y < 0) return new Vector3Int(-1, 0,-1);

        return Vector3Int.zero;
    }
    public static Vector3Int GetWorldVoxelCoordsFromPosition(Vector3 position)
    {
        Debug.Log(position +" > "+ new Vector3Int(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt((position.y * 2) + 0.2f),
            Mathf.RoundToInt(position.z)));
        return new Vector3Int(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt((position.y * 2) + 0.2f),
            Mathf.RoundToInt(position.z));
    }
    public static Vector3 GetPositionFromWorldVoxelCoords(Vector3Int coords)
    {
        Debug.Log(coords + ">" + new Vector3(coords.x, coords.y / 2f, coords.z));
        return new Vector3(coords.x, coords.y / 2f, coords.z);
    }
}