using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainFace
{
    private readonly ShapeGenerator _shapeGenerator;

    private readonly Mesh _mesh;
    private readonly int _resolution;

    private readonly Vector3 _localUp;
    private readonly Vector3 _axisA;
    private readonly Vector3 _axisB;


    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        _shapeGenerator = shapeGenerator;
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

        var triIndex = 0;
        for (var y = 0; y < _resolution; y++)
        {
            for (var x = 0; x < _resolution; x++)
            {
                var i = x + y * _resolution;
                var percent = new Vector2(x, y) / decreasedResolution;
                var pointOnUnitCube = _localUp + (percent.x - .5f) * 2 * _axisA + (percent.y - .5f) * 2 * _axisB;
                var pointOnUnitSphere = pointOnUnitCube.normalized;
                var unscaledElevation = _shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);
                vertices[i] = pointOnUnitSphere * _shapeGenerator.GetScaledElevation(unscaledElevation);
                uv[i].y = unscaledElevation;

                if (x != decreasedResolution && y != decreasedResolution)
                {
                    triangles[triIndex] = i;
                    triangles[++triIndex] = i + _resolution + 1;
                    triangles[++triIndex] = i + _resolution;

                    triangles[++triIndex] = i;
                    triangles[++triIndex] = i + 1;
                    triangles[++triIndex] = i + _resolution + 1;

                    ++triIndex;
                }
            }
        }

        /*ParallelEnumerable.Range(0, _resolution)
            .ForAll(y =>
            {
                var triIndex = y * decreasedResolution * 6;
                for (var x = 0; x < decreasedResolution; x++)
                {
                    var i = x + y * _resolution;
                    GenerateUvAndVertex(i, x, y, vertices, uv);

                    if (y == decreasedResolution)
                    {
                        continue;
                    }

                    triangles[triIndex] = i;
                    triangles[++triIndex] = i + _resolution + 1;
                    triangles[++triIndex] = i + _resolution;
                    triangles[++triIndex] = i;
                    triangles[++triIndex] = i + 1;
                    triangles[++triIndex] = i + _resolution + 1;
                    
                    ++triIndex;
                }

                GenerateUvAndVertex(decreasedResolution + y * _resolution, decreasedResolution, y, vertices, uv);
            });*/

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
        _mesh.uv = uv;
    }

    private void GenerateUvAndVertex(int i, int x, int y, IList<Vector3> vertices, Vector2[] uv)
    {
        var percent = new Vector2(x, y) / (_resolution - 1);
        var pointOnUnitCube = _localUp + (percent.x - .5f) * 2 * _axisA + (percent.y - .5f) * 2 * _axisB;
        var pointOnUnitSphere = pointOnUnitCube.normalized;
        var unscaledElevation = _shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);

        vertices[i] = pointOnUnitSphere * _shapeGenerator.GetScaledElevation(unscaledElevation);
        uv[i].y = unscaledElevation;
    }

    public void UpdateUVs(ColorGenerator colorGenerator)
    {
        var decreasedResolution = _resolution - 1;
        var uv = _mesh.uv;

        for (var y = 0; y < _resolution; y++)
        {
            for (var x = 0; x < _resolution; x++)
            {
                var i = x + y * _resolution;
                var percent = new Vector2(x, y) / decreasedResolution;
                var pointOnUnitCube = _localUp + (percent.x - .5f) * 2 * _axisA + (percent.y - .5f) * 2 * _axisB;
                var pointOnUnitSphere = pointOnUnitCube.normalized;

                uv[i].x = colorGenerator.BiomePercentFromPoint(pointOnUnitSphere);
            }
        }

        _mesh.uv = uv;
    }
}