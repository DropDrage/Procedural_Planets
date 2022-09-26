using System;
using Planet.Common;
using Planet.Common.Generation;
using UnityEngine;
using UnityEngine.Profiling;

namespace Planet.Generation_Methods.Singlethreaded
{
    [Obsolete("Use async")]
    public class TerrainFaceGenerator : BaseTerrainFaceGenerator
    {
        public TerrainFaceGenerator(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp) :
            base(shapeGenerator, mesh, resolution, localUp)
        {
        }


        public void ConstructMesh()
        {
            Profiler.BeginSample("ConstructMesh");

            var decreasedResolution = resolution - 1;
            var triangles = new int[decreasedResolution * decreasedResolution * 6];
            var vertices = new Vector3[resolution * resolution];
            var uv = mesh.uv.Length == vertices.Length ? mesh.uv : new Vector2[vertices.Length];

            Profiler.BeginSample("ConstructMesh_Loop");

            var triangleVertexIndex = 0;
            for (var y = 0; y < resolution; y++)
            {
                var yResolution = y * resolution;
                for (var x = 0; x < decreasedResolution; x++)
                {
                    var i = x + yResolution;
                    GenerateUvAndVertex(i, x, y, vertices, uv);

                    if (y == decreasedResolution)
                    {
                        continue;
                    }

                    triangleVertexIndex = SetCell(triangles, i, triangleVertexIndex);
                }

                GenerateUvAndVertex(decreasedResolution + y * resolution, decreasedResolution, y, vertices, uv);
            }

            Profiler.EndSample();

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.uv = uv;

            Profiler.EndSample();
        }

        public void UpdateUVs(ColorGenerator colorGenerator)
        {
            Profiler.BeginSample("UpdateUVs");
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
            Profiler.EndSample();
        }
    }
}
