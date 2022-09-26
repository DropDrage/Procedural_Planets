using UnityEngine;

namespace Planet.Generation_Methods.Multithreaded
{
    public class SunGeneratorAsync : PlanetGeneratorAsync
    {
        private const float MinAudioDistanceModifier = 1.1f;
        private const float MaxAudioDistanceModifier = 15f;


        protected override TerrainFaceGeneratorAsync CreateTerrainFace(Mesh sharedMesh, Vector3 direction) =>
            new FlatTerrainFaceGeneratorAsync(shapeGenerator, sharedMesh, resolution, direction);

        protected override void OnRadiusCalculated(Planet planet, float radius)
        {
            base.OnRadiusCalculated(planet, radius);
            var audioSource = planet.GetComponent<AudioSource>();
            audioSource.minDistance = radius * MinAudioDistanceModifier;
            audioSource.maxDistance = radius * MaxAudioDistanceModifier;
        }

        protected override float CalculateRadius(Planet planet) => planet.shapeSettings.planetRadius * 2;
    }
}
