using Planet.Common;
using UnityEngine;
using Utils.Extensions;

namespace Planet.Generation_Methods.GPU
{
    public class PlanetGeneratorGPU
    {
        private const string CalculateTrianglesKernel = "CalculateTriangles";

        private const int TrianglesStep = 6;
        private const int SidesCount = 6;

        private static readonly int ResolutionParameter = Shader.PropertyToID("_Resolution");
        private static readonly int TrianglesBufferParameter = Shader.PropertyToID("_Triangles");


        protected readonly ShapeGenerator shapeGenerator = new();
        private readonly ColorGeneratorGPU _colorGenerator = new();

        public int resolution = 10;

        private TerrainFaceGeneratorGPU[] _terrainFaces;

        private MeshFilter[] _meshFilters;


        public void GeneratePlanet(Planet planet, ComputeShader trianglesComputeShader)
        {
            Debug.Log("Generate planet start");
            Initialize(planet);
            GenerateMesh(planet, trianglesComputeShader);
            GenerateColours(planet);
            Debug.Log("Generate planet mid");

            // System.GC.Collect();
            Debug.Log("Generate planet end");
        }


        private void Initialize(Planet planet)
        {
            Debug.Log("Initialize start");
            shapeGenerator.UpdateSettings(planet.shapeSettings);
            _colorGenerator.UpdateSettings(planet.colorSettings);

            if (planet.sphereCollider == null)
            {
                planet.sphereCollider = planet.gameObject.AddComponent<SphereCollider>();
            }

            if (_meshFilters.IsNullOrEmpty())
            {
                Debug.Log($"Recreate {_meshFilters}");
                _meshFilters = new MeshFilter[6];
            }

            _terrainFaces = new TerrainFaceGeneratorGPU[6];

            var directions = new[]
            {
                Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back,
            };

            var myTransform = planet.transform;
            for (var i = 0; i < SidesCount; i++)
            {
                Mesh sharedMesh = null;
                if (_meshFilters[i] == null)
                {
                    var meshObj = myTransform.childCount > i && myTransform.GetChild(i).gameObject
                        ? myTransform.GetChild(i).gameObject
                        : new GameObject($"mesh{(FaceRenderMask) i + 1}");
                    var meshObjTransform = meshObj.transform;
                    meshObjTransform.parent = myTransform;
                    meshObjTransform.position = myTransform.position;

                    meshObj.AddComponent<MeshRenderer>();
                    _meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                    sharedMesh = _meshFilters[i].sharedMesh = new Mesh();
                }

                _meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = planet.colorSettings.planetMaterial;

                _terrainFaces[i] =
                    CreateTerrainFace(sharedMesh ? sharedMesh : _meshFilters[i].sharedMesh, directions[i]);
            }

            Debug.Log("Initialize end");
        }

        protected virtual TerrainFaceGeneratorGPU CreateTerrainFace(Mesh sharedMesh, Vector3 direction) =>
            new(shapeGenerator, sharedMesh, resolution, direction);


        private void GenerateMesh(Planet planet, ComputeShader trianglesComputeShader)
        {
            var decreasedResolution = resolution - 1;
            var trianglesCount = decreasedResolution * decreasedResolution * TrianglesStep;
            var allSidesTrianglesCount = trianglesCount * SidesCount;
            var trianglesBuffer = new ComputeBuffer(allSidesTrianglesCount, sizeof(int));

            var calculateTriangles = trianglesComputeShader.FindKernel(CalculateTrianglesKernel);
            trianglesComputeShader.SetInt(ResolutionParameter, resolution);
            trianglesComputeShader.SetBuffer(calculateTriangles, TrianglesBufferParameter, trianglesBuffer);
            trianglesComputeShader.Dispatch(calculateTriangles, resolution, decreasedResolution, SidesCount);

            var tr = new int[allSidesTrianglesCount];
            trianglesBuffer.GetData(tr);
            for (var i = 0; i < _terrainFaces.Length; i++)
            {
                var triangles = new int[trianglesCount];
                trianglesBuffer.GetData(triangles, 0, i * trianglesCount, trianglesCount);
                _terrainFaces[i].ConstructMesh(triangles); //[(i * trianglesCount)..((i + 1) * trianglesCount)]
            }

            trianglesBuffer.Release();

            _colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
            OnRadiusCalculated(planet, CalculateRadius(planet));
        }


        protected virtual float CalculateRadius(Planet planet) =>
            planet.shapeSettings.planetRadius * (1 + shapeGenerator.elevationMinMax.Max);

        protected virtual void OnRadiusCalculated(Planet planet, float radius)
        {
            planet.sphereCollider.radius = radius;
        }


        private void GenerateColours(Planet planet)
        {
            _colorGenerator.UpdateColors();

            for (var i = 0; i < _terrainFaces.Length; i++)
            {
                if (_meshFilters[i].gameObject.activeSelf)
                {
                    _terrainFaces[i].UpdateUVs(_colorGenerator);
                }
            }

            planet.trailRenderer.colorGradient = planet.colorSettings.oceanGradient;
        }
    }
}
