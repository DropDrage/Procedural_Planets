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


        public async Task ConstructMesh(int[] triangles, TaskScheduler main)
        {
            var (vertices, uv) = GenerateUvsAndVertices(await RunAsyncWithScheduler(() => mesh.uv, main));

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
