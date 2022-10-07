using System.Collections.Generic;
using System.Threading.Tasks;
using Planet.Common;
using UnityEngine;
using static Utils.AsyncUtils;

namespace Planet.Generation_Methods.Jobs
{
    public class PlanetGeneratorJob
    {
        private const int SidesCount = 6;

        protected readonly ShapeGenerator shapeGenerator = new();
        private readonly ColorGeneratorJob _colorGenerator = new();

        public int resolution = 10;

        private TerrainFaceGeneratorJob[] _terrainFaces;


        public async Task GeneratePlanet(Planet planet, TaskScheduler main)
        {
            await Initialize(planet, main);
            await GenerateMesh(planet, main);
            await GenerateColours(planet, main);
        }


        private async Task Initialize(Planet planet, TaskScheduler main)
        {
            shapeGenerator.UpdateSettings(planet.shapeSettings);
            await _colorGenerator.UpdateSettings(planet.colorSettings, main);

            if (planet.sphereCollider == null)
            {
                planet.sphereCollider =
                    await RunAsyncWithScheduler(planet.gameObject.AddComponent<SphereCollider>, main);
            }

            var meshFilters = new MeshFilter[SidesCount];
            _terrainFaces = new TerrainFaceGeneratorJob[SidesCount];

            var directions = new[]
            {
                Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back,
            };

            var myTransform = await RunAsyncWithScheduler(() => planet.transform, main);
            for (var i = 0; i < meshFilters.Length; i++)
            {
                Mesh sharedMesh = null;
                MeshRenderer meshRenderer = null;
                await RunAsyncWithScheduler(() =>
                {
                    var meshObj = new GameObject($"mesh{(FaceRenderMask) i + 1}");
                    var meshObjTransform = meshObj.transform;
                    meshObjTransform.parent = myTransform;
                    meshObjTransform.position = myTransform.position;

                    meshRenderer = meshObj.AddComponent<MeshRenderer>();
                    meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                    sharedMesh = meshFilters[i].sharedMesh = new Mesh();
                }, main);

                await RunAsyncWithScheduler(
                    () => meshRenderer.sharedMaterial = planet.colorSettings.planetMaterial,
                    main);

                _terrainFaces[i] = CreateTerrainFace(sharedMesh, directions[i]);
            }
        }

        protected virtual TerrainFaceGeneratorJob CreateTerrainFace(Mesh sharedMesh, Vector3 direction) =>
            new(shapeGenerator, sharedMesh, resolution, direction);


        private async Task GenerateMesh(Planet planet, TaskScheduler main)
        {
            var constructMeshTasks = new Task[_terrainFaces.Length];
            for (var i = 0; i < _terrainFaces.Length; i++)
            {
                constructMeshTasks[i] = _terrainFaces[i].ConstructMesh(main);
            }
            Task.WaitAll(constructMeshTasks);

            await RunAsyncWithScheduler(() =>
            {
                _colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
                OnRadiusCalculated(planet, CalculateRadius(planet));
            }, main);
        }


        protected virtual float CalculateRadius(Planet planet) =>
            planet.shapeSettings.planetRadius * (1 + shapeGenerator.elevationMinMax.Max);

        protected virtual void OnRadiusCalculated(Planet planet, float radius)
        {
            planet.sphereCollider.radius = radius;
        }


        private async Task GenerateColours(Planet planet, TaskScheduler main)
        {
            await _colorGenerator.UpdateColors(main);

            var uvUpdateTasks = new List<Task>(_terrainFaces.Length);
            for (var i = 0; i < _terrainFaces.Length; i++)
            {
                uvUpdateTasks.Add(_terrainFaces[i].UpdateUVs(_colorGenerator, main));
            }
            Task.WaitAll(uvUpdateTasks.ToArray());

            await RunAsyncWithScheduler(
                () => planet.trailRenderer.colorGradient = planet.colorSettings.oceanGradient,
                main);
        }
    }
}
