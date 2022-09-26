using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Planet.Common.Generation
{
    public abstract class BaseTerrainFaceGenerator
    {
        protected const int TrianglesStep = 6;

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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SetCell(IList<int> triangles, int position, int triangleVertexIndex)
        {
            triangles[triangleVertexIndex] = triangles[triangleVertexIndex + 3] = position;
            triangles[triangleVertexIndex + 1] = triangles[triangleVertexIndex + 5] = position + resolution + 1;
            triangles[triangleVertexIndex + 2] = position + resolution;
            triangles[triangleVertexIndex + 4] = position + 1;
        }

        protected Vector3 GeneratePointOnUnitSphere(int x, int y)
        {
            var percent = new Vector2(x / (resolution - 1f), y / (resolution - 1f));
            var pointOnUnitCube = _localUp + (percent.x - .5f) * 2 * _axisA + (percent.y - .5f) * 2 * _axisB;
            return pointOnUnitCube.normalized;
        }

        protected void GenerateUvAndVertex(int i, int x, int y, IList<Vector3> vertices, Vector2[] uv)
        {
            var pointOnUnitSphere = GeneratePointOnUnitSphere(x, y);
            var unscaledElevation = shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);

            vertices[i] = CalculateVertex(pointOnUnitSphere, unscaledElevation);
            uv[i].y = unscaledElevation;
        }

        protected virtual Vector3 CalculateVertex(Vector3 pointOnUnitSphere, float unscaledElevation) =>
            pointOnUnitSphere * shapeGenerator.GetScaledElevation(unscaledElevation);
    }
}
