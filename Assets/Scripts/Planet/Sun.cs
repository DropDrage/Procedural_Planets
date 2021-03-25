using UnityEngine;

public class Sun : Planet
{
    protected override TerrainFace CreateTerrainFace(Mesh sharedMesh, Vector3 direction) =>
        new FlatTerrainFace(ShapeGenerator, sharedMesh, resolution, direction);

    protected override float CalculateRadius() => shapeSettings.planetRadius;
}