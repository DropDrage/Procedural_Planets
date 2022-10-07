using System;
using Planet.Common;
using Planet.Common.Generation;
using UnityEngine;

namespace Planet.Generation_Methods.Singlethreaded
{
    [Obsolete("Use async")]
    public class PlanetGenerator
    {
        private const int SidesCount = 6;

        protected readonly ShapeGenerator shapeGenerator = new();
        private readonly ColorGenerator _colorGenerator = new();

        public int resolution = 10;

        private TerrainFaceGenerator[] _terrainFaces;


        public void GeneratePlanet(Planet planet)
        {
            Initialize(planet);
            GenerateMesh(planet);
            GenerateColours(planet);

            planet.GetComponent<GravityBody>().enabled = true;
        }


        private void Initialize(Planet planet)
        {
            shapeGenerator.UpdateSettings(planet.shapeSettings);
            _colorGenerator.UpdateSettings(planet.colorSettings);

            var meshFilters = new MeshFilter[SidesCount];
            _terrainFaces = new TerrainFaceGenerator[SidesCount];

            var directions = new[]
            {
                Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back,
            };

            var myTransform = planet.transform;
            for (var i = 0; i < meshFilters.Length; i++)
            {
                var meshObj = new GameObject($"mesh{(FaceRenderMask) i + 1}");
                var meshObjTransform = meshObj.transform;
                meshObjTransform.parent = myTransform;
                meshObjTransform.position = myTransform.position;

                meshObj.AddComponent<MeshRenderer>();
                meshObj.AddComponent<MeshFilter>();

                meshFilters[i] = meshObj.GetComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
                meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = planet.colorSettings.planetMaterial;

                _terrainFaces[i] = CreateTerrainFace(meshFilters[i].sharedMesh, directions[i]);
            }
        }

        protected virtual TerrainFaceGenerator CreateTerrainFace(Mesh sharedMesh, Vector3 direction) =>
            new(shapeGenerator, sharedMesh, resolution, direction);

        private void GenerateMesh(Planet planet)
        {
            var triangles = BaseTerrainFaceGenerator.GetTriangles(resolution);
            foreach (var terrainFace in _terrainFaces)
            {
                terrainFace.ConstructMesh(triangles);
            }

            _colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
            OnRadiusCalculated(planet, CalculateRadius(planet));
        }

        protected virtual void OnRadiusCalculated(Planet planet, float radius)
        {
            planet.sphereCollider.radius = radius;
        }

        private void GenerateColours(Planet planet)
        {
            _colorGenerator.UpdateColors();
            foreach (var terrainFace in _terrainFaces)
            {
                terrainFace.UpdateUVs(_colorGenerator);
            }

            planet.trailRenderer.colorGradient = planet.colorSettings.oceanGradient;
        }


        protected virtual float CalculateRadius(Planet planet) =>
            planet.shapeSettings.planetRadius * (1 + shapeGenerator.elevationMinMax.Max);
    }
}
