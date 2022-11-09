using Cysharp.Threading.Tasks;
using Planet.Common;
using Planet.Common.Generation;
using UnityEngine;
using static Utils.UniTaskUtils;

namespace Planet.Generation_Methods.Multithreaded_UniTask
{
    public class TerrainFaceGeneratorUniTask : BaseTerrainFaceGenerator
    {
        public TerrainFaceGeneratorUniTask(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp) :
            base(shapeGenerator, mesh, resolution, localUp)
        {
        }


        public async UniTask ConstructMesh(int[] triangles)
        {
            var uvAndVertices = GenerateUvsAndVertices(await RunOnMainThreadFromThreadPool(() => mesh.uv));

            await RunOnMainThreadFromThreadPool(
                input =>
                {
                    mesh.Clear();
                    mesh.vertices = input.vertices;
                    mesh.triangles = triangles;
                    mesh.RecalculateNormals();
                    mesh.uv = input.uv;
                }, uvAndVertices
            );
        }


        public async UniTask UpdateUVs(ColorGeneratorUniTask colorGenerator)
        {
            var uv = await RunOnMainThreadFromThreadPool(() => mesh.uv);

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

            await RunOnMainThreadFromThreadPool(uv => mesh.uv = uv, uv);
        }
    }
}
