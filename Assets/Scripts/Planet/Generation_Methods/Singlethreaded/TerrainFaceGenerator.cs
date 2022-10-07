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


        public void ConstructMesh(int[] triangles)
        {
            Profiler.BeginSample("ConstructMesh");

            var (vertices, uv) = GenerateUvsAndVertices(mesh.uv);

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
