using Planet.Common;
using UnityEngine;

namespace Planet.Generation_Methods.GPU
{
    public class FlatTerrainFaceGeneratorGPU : TerrainFaceGeneratorGPU
    {
        public FlatTerrainFaceGeneratorGPU(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
            : base(shapeGenerator, mesh, resolution, localUp)
        {
        }

        protected override Vector3 CalculateVertex(ref Vector3 pointOnUnitSphere, float unscaledElevation) =>
            pointOnUnitSphere * shapeGenerator.GetScaledElevation(1);
    }
}
