using UnityEngine;

public class SunGenerator : PlanetGenerator
{
    private const float MinAudioDistanceModifier = 1.1f;
    private const float MaxAudioDistanceModifier = 15f;


    protected override TerrainFaceGenerator CreateTerrainFace(Mesh sharedMesh, Vector3 direction) =>
        new FlatTerrainFaceGenerator(shapeGenerator, sharedMesh, resolution, direction);

    protected override void OnRadiusCalculated(float radius)
    {
        base.OnRadiusCalculated(radius);
        var audioSource = GetComponent<AudioSource>();
        audioSource.minDistance = radius * MinAudioDistanceModifier;
        audioSource.maxDistance = radius * MaxAudioDistanceModifier;
    }

    protected override float CalculateRadius() => shapeSettings.planetRadius * 2;
}