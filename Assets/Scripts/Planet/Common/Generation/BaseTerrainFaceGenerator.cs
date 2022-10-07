using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Profiling;

namespace Planet.Common.Generation
{
    public abstract class BaseTerrainFaceGenerator
    {
        private const int TrianglesStep = 6;

        protected readonly ShapeGenerator shapeGenerator;

        protected readonly Mesh mesh;
        protected readonly int resolution;

        private readonly Vector3 _localUp;
        private readonly Vector3 _axisA;
        private readonly Vector3 _axisB;


        protected BaseTerrainFaceGenerator(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
        {
            this.shapeGenerator = shapeGenerator;
            this.mesh = mesh;
            this.resolution = resolution;
            _localUp = localUp;

            _axisA = new Vector3(_localUp.y, _localUp.z, _localUp.x);
            _axisB = Vector3.Cross(_localUp, _axisA);
        }


        protected Vector3 GeneratePointOnUnitSphere(int x, int y)
        {
            var percent = new Vector2(x / (resolution - 1f), y / (resolution - 1f));
            var pointOnUnitCube = _localUp + (percent.x - .5f) * 2 * _axisA + (percent.y - .5f) * 2 * _axisB;
            return pointOnUnitCube.normalized;
        }


        protected (Vector3[] vertices, Vector2[] uv) GenerateUvsAndVertices(Vector2[] meshUv)
        {
            Profiler.BeginSample(nameof(GenerateUvsAndVertices));

            var vertices = new Vector3[resolution * resolution];
            var uv = meshUv.Length == vertices.Length ? meshUv : new Vector2[vertices.Length];

            for (var y = 0; y < resolution; y++)
            {
                var yResolution = y * resolution;
                for (var x = 0; x < resolution; x++)
                {
                    var i = x + yResolution;
                    GenerateUvAndVertex(i, x, y, vertices, uv);
                }
            }

            Profiler.EndSample();

            return (vertices, uv);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GenerateUvAndVertex(int i, int x, int y, IList<Vector3> vertices, Vector2[] uv)
        {
            var pointOnUnitSphere = GeneratePointOnUnitSphere(x, y);
            var unscaledElevation = shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);

            vertices[i] = CalculateVertex(pointOnUnitSphere, unscaledElevation);
            uv[i].y = unscaledElevation;
        }

        protected virtual Vector3 CalculateVertex(Vector3 pointOnUnitSphere, float unscaledElevation) =>
            pointOnUnitSphere * shapeGenerator.GetScaledElevation(unscaledElevation);


        public static int[] GetTriangles(int resolution)
        {
            var decreasedResolution = resolution - 1;

            var triangleVertexIndex = 0;
            var triangles = new int[decreasedResolution * decreasedResolution * TrianglesStep];

            for (int i = 0, positionEnd = decreasedResolution * resolution; i < positionEnd; i++)
            {
                if (i % resolution != decreasedResolution)
                {
                    SetCell(triangles, i, triangleVertexIndex, resolution);
                    triangleVertexIndex += TrianglesStep;
                }
            }

            return triangles;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetCell(IList<int> triangles, int position, int triangleVertexIndex, int resolution)
        {
            triangles[triangleVertexIndex] = triangles[triangleVertexIndex + 3] = position;
            triangles[triangleVertexIndex + 1] = triangles[triangleVertexIndex + 5] = position + resolution + 1;
            triangles[triangleVertexIndex + 2] = position + resolution;
            triangles[triangleVertexIndex + 4] = position + 1;
        }
    }
}
