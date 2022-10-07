using System.Threading.Tasks;
using Planet.Common;
using Planet.Common.Generation;
using UnityEngine;
using static Utils.AsyncUtils;

namespace Planet.Generation_Methods.Jobs
{
    public class TerrainFaceGeneratorJob : BaseTerrainFaceGenerator
    {
        public TerrainFaceGeneratorJob(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp) :
            base(shapeGenerator, mesh, resolution, localUp)
        {
        }


        public async Task ConstructMesh(TaskScheduler main)
        {
            var decreasedResolution = resolution - 1;
            var vertices = new Vector3[resolution * resolution];
            var triangles = new int[decreasedResolution * decreasedResolution * TrianglesStep];
            var meshUv = await RunAsyncWithScheduler(() => mesh.uv, main);
            var uv = meshUv.Length == vertices.Length ? meshUv : new Vector2[vertices.Length];

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

                    SetCell(triangles, i, triangleVertexIndex);
                    triangleVertexIndex += 6;
                }

                GenerateUvAndVertex(decreasedResolution + y * resolution, decreasedResolution, y, vertices, uv);
            }

            await RunAsyncWithScheduler(() =>
            {
                mesh.Clear();
                mesh.vertices = vertices;
                mesh.triangles = triangles;
                mesh.RecalculateNormals();
                mesh.uv = uv;
            }, main);
        }


        public async Task UpdateUVs(ColorGeneratorJob colorGenerator, TaskScheduler main)
        {
            var uv = await RunAsyncWithScheduler(() => mesh.uv, main);

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

            await RunAsyncWithScheduler(() => mesh.uv = uv, main);
        }
    }
}