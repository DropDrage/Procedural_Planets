using System.Linq;
using Settings;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 10;

    public enum FaceRenderMask
    {
        All, Top, Bottom, Left, Right, Front, Back,
    }

    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    private ShapeGenerator _shapeGenerator = new ShapeGenerator();
    private ColorGenerator _colorGenerator = new ColorGenerator();

    [SerializeField, HideInInspector]
    private MeshFilter[] meshFilters;

    private TerrainFace[] _terrainFaces;


    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void OnShapeSettingsUpdated()
    {
        Initialize();
        GenerateMesh();
    }

    public void OnColorSettingsUpdated()
    {
        Initialize();
        GenerateColours();
    }


    private void Initialize()
    {
        _shapeGenerator.UpdateSettings(shapeSettings);
        _colorGenerator.UpdateSettings(colorSettings);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }

        _terrainFaces = new TerrainFace[6];

        var directions = new[]
        {
            Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back,
        };

        for (var i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                var meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

            _terrainFaces[i] = new TerrainFace(_shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
            var renderFace = faceRenderMask == FaceRenderMask.All || (int) faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }

    private void GenerateMesh()
    {
        foreach (var face in _terrainFaces.Where((face, i) => meshFilters[i].gameObject.activeSelf))
        {
            face.ConstructMesh();
        }

        _colorGenerator.UpdateElevation(_shapeGenerator.ElevationMinMax);
    }

    private void GenerateColours()
    {
        _colorGenerator.UpdateColors();
        foreach (var face in _terrainFaces.Where((face, i) => meshFilters[i].gameObject.activeSelf))
        {
            face.UpdateUVs(_colorGenerator);
        }
    }
}