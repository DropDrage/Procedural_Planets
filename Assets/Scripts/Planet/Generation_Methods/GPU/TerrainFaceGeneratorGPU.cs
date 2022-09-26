using System.Collections.Generic;
using Planet.Common;
using UnityEngine;

namespace Planet.Generation_Methods.GPU
{
    public class TerrainFaceGeneratorGPU
    {
        public TerrainFaceGeneratorGPU(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
        {
            this.shapeGenerator = shapeGenerator;
            this.mesh = mesh;
            this.resolution = resolution;
            this.localUp = localUp;

            axisA = new Vector3(this.localUp.y, this.localUp.z, this.localUp.x);
            axisB = Vector3.Cross(this.localUp, axisA);
        }


        public void ConstructMesh(int[] triangles)
        {
            var decreasedResolution = resolution - 1;
            var vertices = new Vector3[resolution * resolution];
            var meshUv = mesh.uv;
            var uv = meshUv.Length == vertices.Length ? meshUv : new Vector2[vertices.Length];

            for (var y = 0; y < resolution; y++)
            {
                var lineStartIndex = y * resolution;
                for (var x = 0; x < decreasedResolution; x++)
                {
                    var i = x + lineStartIndex;
                    GenerateUvAndVertex(i, x, y, vertices, uv);
                }

                GenerateUvAndVertex(decreasedResolution + y * resolution, decreasedResolution, y, vertices, uv);
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.uv = uv;
        }


        public void UpdateUVs(ColorGeneratorGPU colorGenerator)
        {
            var uv = mesh.uv;

            for (var y = 0; y < resolution; y++)
            {
                var yResolution = y * resolution;
                for (var x = 0; x < resolution; x++)
                {
                    var i = x + yResolution;
                    var pointOnUnitSphere = GeneratePointOnUnitSphere(x, y);

                    uv[i].x = colorGenerator.BiomePercentFromPoint(pointOnUnitSphere);
                }
            }

            mesh.uv = uv;
        }


        protected readonly ShapeGenerator shapeGenerator;

        protected readonly Mesh mesh;
        protected readonly int resolution;

        protected readonly Vector3 localUp;
        protected readonly Vector3 axisA;
        protected readonly Vector3 axisB;

        protected Vector3 GeneratePointOnUnitSphere(int x, int y)
        {
            var percent = new Vector2(x / (resolution - 1f), y / (resolution - 1f));
            var pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
            return pointOnUnitCube.normalized;
        }

        protected void GenerateUvAndVertex(int i, int x, int y, IList<Vector3> vertices, Vector2[] uv)
        {
            var pointOnUnitSphere = GeneratePointOnUnitSphere(x, y);
            var unscaledElevation = shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);

            vertices[i] = CalculateVertex(ref pointOnUnitSphere, unscaledElevation);
            uv[i].y = unscaledElevation;
        }

        protected virtual Vector3 CalculateVertex(ref Vector3 pointOnUnitSphere, float unscaledElevation) =>
            pointOnUnitSphere * shapeGenerator.GetScaledElevation(unscaledElevation);
    }
}
