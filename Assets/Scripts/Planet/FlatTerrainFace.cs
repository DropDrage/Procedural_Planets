using UnityEngine;

public class FlatTerrainFace : TerrainFace
{
    public FlatTerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
        : base(shapeGenerator, mesh, resolution, localUp)
    {
    }

    protected override Vector3 CalculateVertex(Vector3 pointOnUnitSphere, float unscaledElevation) =>
        pointOnUnitSphere * ShapeGenerator.GetScaledElevation(1);
}