using UnityEngine;

public static class VectorMath
{
    public static Vector3 ConvertInputVectorUsingCamera(float horizontalInput, float verticalInput, Transform camera)
    {
        Vector3 camForward = camera.forward;
        Vector3 camRight = camera.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        return ((camForward * verticalInput) + (camRight * horizontalInput)).normalized;
    }
    public static Vector2 ConvertInputVectorUsingCamera(Vector2 input, Transform camera)
    {
        Vector3 dir = ConvertInputVectorUsingCamera(input.x, input.y, camera);
        return new Vector2(dir.x, dir.z);
    }

    public static Vector3 RoundNormalizedVectorTo45DegreeGridMovement(Vector3 dir)
    {
        if (dir.x >  0.92f) return new Vector3( 1, 0, 0);
        if (dir.z >  0.92f) return new Vector3( 0, 0, 1);
        if (dir.x < -0.92f) return new Vector3(-1, 0, 0);
        if (dir.z < -0.92f) return new Vector3( 0, 0,-1);

        if (dir.x > 0 && dir.z > 0) return new Vector3( 1, 0, 1);
        if (dir.x < 0 && dir.z > 0) return new Vector3(-1, 0, 1);
        if (dir.x > 0 && dir.z < 0) return new Vector3( 1, 0,-1);
        if (dir.x < 0 && dir.z < 0) return new Vector3(-1, 0,-1);

        return Vector3.zero;
    }
}