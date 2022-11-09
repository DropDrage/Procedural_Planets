using Planet.Common;
using UnityEngine;

namespace Planet.Generation_Methods.Multithreaded_UniTask
{
    public class FlatTerrainFaceGeneratorUniTask : TerrainFaceGeneratorUniTask
    {
        public FlatTerrainFaceGeneratorUniTask(ShapeGenerator shapeGenerator, Mesh mesh, int resolution,
            Vector3 localUp)
            : base(shapeGenerator, mesh, resolution, localUp)
        {
        }

        protected override Vector3 CalculateVertex(Vector3 pointOnUnitSphere, float unscaledElevation) =>
            pointOnUnitSphere * shapeGenerator.GetScaledElevation(1);
    }
}
