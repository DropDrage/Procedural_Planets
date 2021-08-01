using System.Linq;
using Settings;
using UnityEngine;

public class Planet : MonoBehaviour
{
    protected readonly ShapeGenerator shapeGenerator = new ShapeGenerator();
    private readonly ColorGenerator _colorGenerator = new ColorGenerator();

    public enum FaceRenderMask
    {
        All, Top, Bottom, Left, Right, Front, Back,
    }

    [Range(2, 256)]
    public int resolution = 10;

    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [SerializeField, HideInInspector]
    private MeshFilter[] meshFilters;

    private TerrainFace[] _terrainFaces;

    private SphereCollider _sphereCollider;
    private TrailRenderer _trailRenderer;


    private void Start()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _trailRenderer = GetComponent<TrailRenderer>();

        GeneratePlanet();
    }


    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();

        System.GC.Collect();
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
        shapeGenerator.UpdateSettings(shapeSettings);
        _colorGenerator.UpdateSettings(colorSettings);

        if (_sphereCollider == null)
        {
            _sphereCollider = gameObject.AddComponent<SphereCollider>();
        }

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
                    : new GameObject($"mesh{(FaceRenderMask) i + 1}");
                meshObj.transform.parent = transform1;
                meshObj.transform.position = transform1.position;

                meshObj.AddComponent<MeshRenderer>();
                meshObj.AddComponent<MeshFilter>();
                meshFilters[i] = meshObj.GetComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

            _terrainFaces[i] = CreateTerrainFace(meshFilters[i].sharedMesh, directions[i]);
            var renderFace = faceRenderMask == FaceRenderMask.All || (int) faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }

    protected virtual TerrainFace CreateTerrainFace(Mesh sharedMesh, Vector3 direction) =>
        new TerrainFace(shapeGenerator, sharedMesh, resolution, direction);

    private void GenerateMesh()
    {
        foreach (var face in _terrainFaces.Where((face, i) => meshFilters[i].gameObject.activeSelf))
        {
            face.ConstructMesh();
        }

        _colorGenerator.UpdateElevation(shapeGenerator.ElevationMinMax);
        OnRadiusCalculated(CalculateRadius());
    }

    protected virtual void OnRadiusCalculated(float radius)
    {
        _sphereCollider.radius = radius;
    }

    private void GenerateColours()
    {
        _colorGenerator.UpdateColors();
        foreach (var face in _terrainFaces.Where((face, i) => meshFilters[i].gameObject.activeSelf))
        {
            face.UpdateUVs(_colorGenerator);
        }

        _trailRenderer.colorGradient = colorSettings.oceanGradient;
    }


    protected virtual float CalculateRadius() => shapeSettings.planetRadius * (1 + shapeGenerator.ElevationMinMax.Max);
}