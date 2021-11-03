using UnityEngine;

public static class MeshUtils
{
    public static Mesh CreateTransform(Vector3[] v)
    {
        var mesh = new Mesh
        {
            vertices = v,
            triangles = new int[] { 0, 1, 2, 0, 2, 3 }
        };

        var shiftedPositions = new Vector2[] { Vector2.zero, new Vector2(0, v[1].y - v[0].y), new Vector2(v[2].x - v[1].x, v[2].y - v[3].y), new Vector2(v[3].x - v[0].x, 0) };
        mesh.uv = shiftedPositions;

        var widthsHeights = new Vector2[4];
        widthsHeights[0].x = widthsHeights[3].x = shiftedPositions[3].x;
        widthsHeights[1].x = widthsHeights[2].x = shiftedPositions[2].x;
        widthsHeights[0].y = widthsHeights[1].y = shiftedPositions[1].y;
        widthsHeights[2].y = widthsHeights[3].y = shiftedPositions[2].y;
        mesh.uv2 = widthsHeights;

        mesh.UploadMeshData(false);
        return mesh;
    }
}