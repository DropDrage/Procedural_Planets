using System.Collections.Generic;
using System.Threading.Tasks;
using Planet.Common;
using UnityEngine;
using Utils.Extensions;
using static Utils.AsyncUtils;

namespace Planet.Generation_Methods.Multithreaded
{
    public class PlanetAutoGeneratorAsync : Planet
    {
        protected readonly ShapeGenerator shapeGenerator = new();
        private readonly ColorGeneratorAsync _colorGenerator = new();

        [Range(2, 256)] public int resolution = 10;

        [SerializeField, HideInInspector] private MeshFilter[] meshFilters;

        private TerrainFaceGeneratorAsync[] _terrainFaces;

        public bool generateOnStart = true;


        private async void Start()
        {
            if (generateOnStart)
            {
                await GeneratePlanet(TaskScheduler.FromCurrentSynchronizationContext());

                GetComponent<GravityBody>().enabled = true;
            }
        }


        public async Task GeneratePlanet(TaskScheduler main)
        {
            Debug.Log("Generate planet start");
            await Initialize(main);
            await GenerateMesh(main);
            await GenerateColours(main);
            Debug.Log("Generate planet mid");

            // System.GC.Collect();
            Debug.Log("Generate planet end");
        }


        private async Task Initialize(TaskScheduler main)
        {
            Debug.Log("Initialize start");
            shapeGenerator.UpdateSettings(shapeSettings);
            await _colorGenerator.UpdateSettings(colorSettings, main);

            if (sphereCollider == null)
            {
                sphereCollider = await RunAsyncWithScheduler(gameObject.AddComponent<SphereCollider>, main);
            }

            if (meshFilters.IsNullOrEmpty())
            {
                print($"Recreate {meshFilters}");
                meshFilters = new MeshFilter[6];
            }

            _terrainFaces = new TerrainFaceGeneratorAsync[6];

            var directions = new[]
            {
                Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back,
            };

            var myTransform = await RunAsyncWithScheduler(() => transform, main);
            for (var i = 0; i < 6; i++)
            {
                Mesh sharedMesh = null;
                if (meshFilters[i] == null)
                {
                    await RunAsyncWithScheduler(() =>
                    {
                        var meshObj = myTransform.childCount > i && myTransform.GetChild(i).gameObject
                            ? myTransform.GetChild(i).gameObject
                            : new GameObject($"mesh{(FaceRenderMask) i + 1}");
                        var meshObjTransform = meshObj.transform;
                        meshObjTransform.parent = myTransform;
                        meshObjTransform.position = myTransform.position;

                        meshObj.AddComponent<MeshRenderer>();
                        meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                        sharedMesh = meshFilters[i].sharedMesh = new Mesh();
                    }, main);
                }

                await RunAsyncWithScheduler(
                    () => meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial,
                    main);

                _terrainFaces[i] = CreateTerrainFace(
                    sharedMesh ? sharedMesh : await RunAsyncWithScheduler(() => meshFilters[i].sharedMesh, main),
                    directions[i]);
                var isFaceRender = faceRenderMask == FaceRenderMask.All || (int) faceRenderMask - 1 == i;
                await RunAsyncWithScheduler(() => meshFilters[i].gameObject.SetActive(isFaceRender), main);
            }

            Debug.Log("Initialize end");
        }

        protected virtual TerrainFaceGeneratorAsync CreateTerrainFace(Mesh sharedMesh, Vector3 direction) =>
            new(shapeGenerator, sharedMesh, resolution, direction);


        private async Task GenerateMesh(TaskScheduler main)
        {
            var meshConstructTasks = new List<Task>(_terrainFaces.Length);
            for (var i = 0; i < _terrainFaces.Length; i++)
            {
                if (await RunAsyncWithScheduler(() => meshFilters[i].gameObject.activeSelf, main))
                {
                    meshConstructTasks.Add(_terrainFaces[i].ConstructMesh(main));
                }
            }
            Task.WaitAll(meshConstructTasks.ToArray());

            await RunAsyncWithScheduler(() =>
            {
                _colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
                OnRadiusCalculated(CalculateRadius());
            }, main);
        }


        protected virtual float CalculateRadius() =>
            shapeSettings.planetRadius * (1 + shapeGenerator.elevationMinMax.Max);

        protected virtual void OnRadiusCalculated(float radius)
        {
            sphereCollider.radius = radius;
        }


        private async Task GenerateColours(TaskScheduler main)
        {
            await _colorGenerator.UpdateColors(main);

            var uvUpdateTasks = new List<Task>(_terrainFaces.Length);
            for (var i = 0; i < _terrainFaces.Length; i++)
            {
                if (await RunAsyncWithScheduler(() => meshFilters[i].gameObject.activeSelf, main))
                {
                    uvUpdateTasks.Add(_terrainFaces[i].UpdateUVs(_colorGenerator, main));
                }
            }
            Task.WaitAll(uvUpdateTasks.ToArray());

            await RunAsyncWithScheduler(() => trailRenderer.colorGradient = colorSettings.oceanGradient, main);
        }
    }
}
