using System.Linq;
using Settings;
using UnityEngine;

public class Planet : MonoBehaviour
{
    private readonly ShapeGenerator _shapeGenerator = new ShapeGenerator();
    private readonly ColorGenerator _colorGenerator = new ColorGenerator();

    [Range(2, 256)]
    public int resolution = 10;

    public enum FaceRenderMask
    {
        All, Top, Bottom, Left, Right, Front, Back,
    }

    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [SerializeField, HideInInspector]
    private MeshFilter[] meshFilters;

    private TerrainFace[] _terrainFaces;

    private SphereCollider _sphereCollider;

    [SerializeField] private Vector3 spawnPoint;


    private void Start()
    {
        spawnPoint = transform.position;
        _sphereCollider = GetComponent<SphereCollider>();
        GeneratePlanet();
    }


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

        if (_sphereCollider == null)
        {
            _sphereCollider = gameObject.AddComponent<SphereCollider>();
        }

        // gameObject.AddComponent<SphereCollider>().radius = shapeSettings.planetRadius;

        if (meshFilters == null || meshFilters.Length == 0)
        {
            print($"Recreate {meshFilters}");
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
                var transform1 = transform;
                var meshObj = transform1.childCount > i && transform1.GetChild(i).gameObject
                    ? transform1.GetChild(i).gameObject
                    : new GameObject($"mesh{i}");
                meshObj.transform.parent = transform1;
                meshObj.transform.position = transform1.position;

                meshObj.AddComponent<MeshRenderer>();
                meshObj.AddComponent<MeshFilter>();
                meshFilters[i] = meshObj.GetComponent<MeshFilter>();
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

        var planetMaxRadius = shapeSettings.planetRadius * (1 + _shapeGenerator.ElevationMinMax.Max);
        _sphereCollider.radius = planetMaxRadius;
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