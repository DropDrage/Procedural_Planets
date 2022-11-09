using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Profiling;

namespace Planet.Common.Generation
{
    public abstract class BaseTerrainFaceGenerator
    {
        private const int TrianglesStep = 6;
        private const int TrianglesSidesCount = 3;

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
            Profiler.BeginSample(nameof(GenerateUvAndVertex));

            var pointOnUnitSphere = GeneratePointOnUnitSphere(x, y);
            var unscaledElevation = shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);

            vertices[i] = CalculateVertex(pointOnUnitSphere, unscaledElevation);
            uv[i].y = unscaledElevation;

            Profiler.EndSample();
        }

        protected virtual Vector3 CalculateVertex(Vector3 pointOnUnitSphere, float unscaledElevation) =>
            pointOnUnitSphere * shapeGenerator.GetScaledElevation(unscaledElevation);


        #region Normals Manual Calculation

        [Obsolete("Much slower than mesh.RecalculateNormals")]
        protected Vector3[] CalculateNormals(Vector3[] vertices)
        {
            var decreasedResolution = resolution - 1;
            var normals = new Vector3[vertices.Length];
            for (int i = 0, cellsCount = normals.Length - resolution; i < cellsCount; i++)
            {
                if (i % resolution != decreasedResolution)
                {
                    var cellStartIndex = i;
                    var cellNormals = CalculateNormal(vertices, cellStartIndex, resolution);
                    AddNormals(normals, ref cellNormals, cellStartIndex, resolution);
                }
            }

            foreach (var normal in normals)
            {
                normal.Normalize();
            }

            return normals;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (Vector3 normal1, Vector3 normal2) CalculateNormal(Vector3[] vertices, int cellStartIndex,
            int resolution)
        {
            ref Vector3 a = ref vertices[cellStartIndex],
                b = ref vertices[cellStartIndex + resolution + 1],
                c = ref vertices[cellStartIndex + resolution];

            Vector3 sideAb = b - a, sideAc = c - a;
            var normal1 = Vector3.Cross(sideAb, sideAc).normalized;

            ref var c2 = ref vertices[cellStartIndex + 1]; // 1
            var sideAc2 = a - c2;
            var normal2 = Vector3.Cross(sideAb, sideAc2).normalized;

            return (normal1, normal2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddNormals(IList<Vector3> normals, ref (Vector3 normal1, Vector3 normal2) cellNormals,
            int cellStartIndex, int resolution)
        {
            var normalsSum = cellNormals.normal1 + cellNormals.normal2;
            normals[cellStartIndex] += normalsSum;
            normals[cellStartIndex + resolution + 1] += normalsSum;
            normals[cellStartIndex + resolution] += cellNormals.normal1;

            normals[cellStartIndex + 1] += cellNormals.normal2;
        }

        #endregion


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
