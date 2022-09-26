using System;
using Planet.Common;
using UnityEngine;

namespace Planet.Generation_Methods.Singlethreaded
{
    [Obsolete("Use async")]
    public class PlanetAutoGenerator : Planet
    {
        protected readonly ShapeGenerator shapeGenerator = new();
        private readonly ColorGenerator _colorGenerator = new();

        [Range(2, 256)] public int resolution = 10;

        [SerializeField, HideInInspector] private MeshFilter[] meshFilters;

        private TerrainFaceGenerator[] _terrainFaces;

        public bool generateOnStart = true;


        private void Start()
        {
            sphereCollider = GetComponent<SphereCollider>();
            trailRenderer = GetComponent<TrailRenderer>();

            if (generateOnStart)
            {
                GeneratePlanet();
            }
        }


        public void GeneratePlanet()
        {
            Initialize();
            GenerateMesh();
            GenerateColours();

            GetComponent<GravityBody>().enabled = true;
            // System.GC.Collect();
        }

        public void OnShapeSettingsUpdated()
        {
            Initialize();
            GenerateMesh();
        }

        public void OnColorSettingsUpdated()
        {
            Initialize();
            GenerateColours();
        }


        private void Initialize()
        {
            shapeGenerator.UpdateSettings(shapeSettings);
            _colorGenerator.UpdateSettings(colorSettings);

            if (sphereCollider == null)
            {
                sphereCollider = gameObject.AddComponent<SphereCollider>();
            }

            if (meshFilters == null || meshFilters.Length == 0)
            {
                print($"Recreate {meshFilters}");
                meshFilters = new MeshFilter[6];
            }

            _terrainFaces = new TerrainFaceGenerator[6];

            var directions = new[]
            {
                Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back,
            };

            var myTransform = transform;
            for (var i = 0; i < 6; i++)
            {
                if (meshFilters[i] == null)
                {
                    var meshObj = myTransform.childCount > i && myTransform.GetChild(i).gameObject
                        ? myTransform.GetChild(i).gameObject
                        : new GameObject($"mesh{(FaceRenderMask) i + 1}");
                    var meshObjTransform = meshObj.transform;
                    meshObjTransform.parent = myTransform;
                    meshObjTransform.position = myTransform.position;

                    meshObj.AddComponent<MeshRenderer>();
                    meshObj.AddComponent<MeshFilter>();
                    meshFilters[i] = meshObj.GetComponent<MeshFilter>();
                    meshFilters[i].sharedMesh = new Mesh();
                }

                meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

                _terrainFaces[i] = CreateTerrainFace(meshFilters[i].sharedMesh, directions[i]);
                var renderFace = faceRenderMask == FaceRenderMask.All || (int) faceRenderMask - 1 == i;
                meshFilters[i].gameObject.SetActive(renderFace);
            }
        }

        protected virtual TerrainFaceGenerator CreateTerrainFace(Mesh sharedMesh, Vector3 direction) =>
            new(shapeGenerator, sharedMesh, resolution, direction);

        private void GenerateMesh()
        {
            for (var i = 0; i < _terrainFaces.Length; i++)
            {
                if (meshFilters[i].gameObject.activeSelf)
                {
                    _terrainFaces[i].ConstructMesh();
                }
            }

            _colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
            OnRadiusCalculated(CalculateRadius());
        }

        protected virtual void OnRadiusCalculated(float radius)
        {
            sphereCollider.radius = radius;
        }

        private void GenerateColours()
        {
            _colorGenerator.UpdateColors();
            for (var i = 0; i < _terrainFaces.Length; i++)
            {
                if (meshFilters[i].gameObject.activeSelf)
                {
                    _terrainFaces[i].UpdateUVs(_colorGenerator);
                }
            }

            trailRenderer.colorGradient = colorSettings.oceanGradient;
        }


        protected virtual float CalculateRadius() =>
            shapeSettings.planetRadius * (1 + shapeGenerator.elevationMinMax.Max);
    }
}
