using Planet.Common;
using UnityEngine;

namespace Planet.Generation
{
    public class FlatTerrainFaceGenerator : TerrainFaceGenerator
    {
        public FlatTerrainFaceGenerator(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
            : base(shapeGenerator, mesh, resolution, localUp)
        {
        }

        protected override Vector3 CalculateVertex(Vector3 pointOnUnitSphere, float unscaledElevation) =>
            pointOnUnitSphere * shapeGenerator.GetScaledElevation(1);
    }
}
