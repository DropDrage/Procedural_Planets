using Cysharp.Threading.Tasks;
using Planet.Common;
using Planet.Common.Generation;
using UnityEngine;
using static Utils.UniTaskUtils;

namespace Planet.Generation_Methods.Multithreaded_UniTask
{
    public class PlanetGeneratorUniTask
    {
        private const int SidesCount = 6;

        protected readonly ShapeGenerator shapeGenerator = new ();
        private readonly ColorGeneratorUniTask _colorGenerator = new ();

        public int resolution = 10;

        private TerrainFaceGeneratorUniTask[] _terrainFaces;


        public async UniTask GeneratePlanet(Planet planet)
        {
            await Initialize(planet);
            await GenerateMesh(planet);
            await GenerateColours(planet);
        }


        private async UniTask Initialize(Planet planet)
        {
            shapeGenerator.UpdateSettings(planet.shapeSettings);
            await _colorGenerator.UpdateSettings(planet.colorSettings);

            var meshFilters = new MeshFilter[SidesCount];
            _terrainFaces = new TerrainFaceGeneratorUniTask[SidesCount];

            var directions = new[]
            {
                Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back,
            };

            var myTransform = await RunOnMainThreadFromThreadPool(() => planet.transform);
            for (var i = 0; i < meshFilters.Length; i++)
            {
                MeshRenderer meshRenderer = null;
                Mesh sharedMesh = null;
                await RunOnMainThreadFromThreadPool(
                    input =>
                    {
                        var (i, myTransform) = input;

                        var meshObj = new GameObject($"mesh{(FaceRenderMask) i + 1}");
                        var meshObjTransform = meshObj.transform;
                        meshObjTransform.parent = myTransform;
                        meshObjTransform.position = myTransform.position;

                        meshRenderer = meshObj.AddComponent<MeshRenderer>();
                        meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                        sharedMesh = meshFilters[i].sharedMesh = new Mesh();
                    }, (i, myTransform)
                );

                await RunOnMainThreadFromThreadPool(
                    meshRenderer => meshRenderer.sharedMaterial = planet.colorSettings.planetMaterial, meshRenderer
                );

                _terrainFaces[i] = CreateTerrainFace(sharedMesh, directions[i]);
            }
        }

        protected virtual TerrainFaceGeneratorUniTask CreateTerrainFace(Mesh sharedMesh, Vector3 direction) =>
            new (shapeGenerator, sharedMesh, resolution, direction);


        private async UniTask GenerateMesh(Planet planet)
        {
            var triangles = BaseTerrainFaceGenerator.GetTriangles(resolution);
            var constructMeshTasks = new UniTask[_terrainFaces.Length];
            for (var i = 0; i < _terrainFaces.Length; i++)
            {
                constructMeshTasks[i] = _terrainFaces[i].ConstructMesh(triangles);
            }
            await UniTask.WhenAll(constructMeshTasks);

            await RunOnMainThreadFromThreadPool(
                planet =>
                {
                    _colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
                    OnRadiusCalculated(planet, CalculateRadius(planet));
                }, planet
            );
        }


        protected virtual float CalculateRadius(Planet planet) =>
            planet.shapeSettings.planetRadius * (1 + shapeGenerator.elevationMinMax.Max);

        protected virtual void OnRadiusCalculated(Planet planet, float radius)
        {
            planet.sphereCollider.radius = radius;
        }


        private async UniTask GenerateColours(Planet planet)
        {
            await _colorGenerator.UpdateColors();

            var uvUpdateTasks = new UniTask[_terrainFaces.Length];
            for (var i = 0; i < _terrainFaces.Length; i++)
            {
                uvUpdateTasks[i] = _terrainFaces[i].UpdateUVs(_colorGenerator);
            }
            await UniTask.WhenAll(uvUpdateTasks);

            await RunOnMainThreadFromThreadPool(
                planet => planet.trailRenderer.colorGradient = planet.colorSettings.oceanGradient, planet
            );
        }
    }
}
