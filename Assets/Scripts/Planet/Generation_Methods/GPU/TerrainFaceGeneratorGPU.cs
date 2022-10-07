using Planet.Common;
using Planet.Common.Generation;
using UnityEngine;

namespace Planet.Generation_Methods.GPU
{
    public class TerrainFaceGeneratorGPU : BaseTerrainFaceGenerator
    {
        public TerrainFaceGeneratorGPU(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp) :
            base(shapeGenerator, mesh, resolution, localUp)
        {
        }


        public void ConstructMesh(int[] triangles)
        {
            var (vertices, uv) = GenerateUvsAndVertices(mesh.uv);

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
    }
}
