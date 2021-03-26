using UnityEngine;

public class Sun : Planet
{
    protected override TerrainFace CreateTerrainFace(Mesh sharedMesh, Vector3 direction) =>
        new FlatTerrainFace(ShapeGenerator, sharedMesh, resolution, direction);

    protected override void OnRadiusCalculated(float radius)
    {
        base.OnRadiusCalculated(radius);
        var audioSource = GetComponent<AudioSource>();
        audioSource.minDistance = radius * 1.1f;
        audioSource.maxDistance = radius * 15f;
    }

    protected override float CalculateRadius() => shapeSettings.planetRadius * 2;
}