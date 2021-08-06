using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainFaceGenerator
{
    protected readonly ShapeGenerator shapeGenerator;

    private readonly Mesh _mesh;
    private readonly int _resolution;

    private readonly Vector3 _localUp;
    private readonly Vector3 _axisA;
    private readonly Vector3 _axisB;


    public TerrainFaceGenerator(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        _mesh = mesh;
        _resolution = resolution;
        _localUp = localUp;

        _axisA = new Vector3(_localUp.y, _localUp.z, _localUp.x);
        _axisB = Vector3.Cross(_localUp, _axisA);
    }

    public void ConstructMesh()
    {
        var decreasedResolution = _resolution - 1;
        var vertices = new Vector3[_resolution * _resolution];
        var triangles = new int[decreasedResolution * decreasedResolution * 6];
        var uv = _mesh.uv.Length == vertices.Length ? _mesh.uv : new Vector2[vertices.Length];

        var triangleVertexIndex = 0;
        for (var y = 0; y < _resolution; y++)
        {
            var yResolution = y * _resolution;
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

            GenerateUvAndVertex(decreasedResolution + y * _resolution, decreasedResolution, y, vertices, uv);
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
        _mesh.uv = uv;
    }

    private int SetCell(IList<int> triangles, int position, int triangleVertexIndex)
    {
        triangles[triangleVertexIndex] = triangles[triangleVertexIndex + 3] = position;
        triangles[triangleVertexIndex + 1] = triangles[triangleVertexIndex + 5] = position + _resolution + 1;
        triangles[triangleVertexIndex + 2] = position + _resolution;
        triangles[triangleVertexIndex + 4] = position + 1;
        return triangleVertexIndex + 6;
    }

    private Vector3 GeneratePointOnUnitSphere(int x, int y)
    {
        var percent = new Vector2(x, y) / (_resolution - 1);
        var pointOnUnitCube = _localUp + (percent.x - .5f) * 2 * _axisA + (percent.y - .5f) * 2 * _axisB;
        return pointOnUnitCube.normalized;
    }

    private void GenerateUvAndVertex(int i, int x, int y, IList<Vector3> vertices, Vector2[] uv)
    {
        var pointOnUnitSphere = GeneratePointOnUnitSphere(x, y);
        var unscaledElevation = shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);

        vertices[i] = CalculateVertex(pointOnUnitSphere, unscaledElevation);
        uv[i].y = unscaledElevation;
    }

    protected virtual Vector3 CalculateVertex(Vector3 pointOnUnitSphere, float unscaledElevation) =>
        pointOnUnitSphere * shapeGenerator.GetScaledElevation(unscaledElevation);

    public void UpdateUVs(ColorGenerator colorGenerator)
    {
        var uv = _mesh.uv;

        ParallelEnumerable.Range(0, _resolution)
            .ForAll(y =>
            {
                var yResolution = y * _resolution;
                for (var x = 0; x < _resolution; x++)
                {
                    var i = x + yResolution;
                    var pointOnUnitSphere = GeneratePointOnUnitSphere(x, y);

                    uv[i].x = colorGenerator.BiomePercentFromPoint(pointOnUnitSphere);
                }
            });

        _mesh.uv = uv;
    }
}