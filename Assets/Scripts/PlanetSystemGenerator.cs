using System.Linq;
using PlanetSystemGenerators;
using UnityEngine;

public class PlanetSystemGenerator : MonoBehaviour
{
    private readonly char[] _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private readonly char[] _alphabetLower = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray();
    private readonly char[] _digits = "1234567890".ToCharArray();


    [SerializeField] private IntRange nameLengthRange;

    [SerializeField] private FloatRange orbitDistanceRadius;
    [SerializeField] private IntRange planetCountRange;

    [SerializeField] private Vector3 center;
    [SerializeField] private PlanetGenerator planetGenerator;
    [SerializeField] private SunGenerator sunGenerator;

    [Space(20)]
    [Range(0, int.MaxValue)]
    [SerializeField] private int seed = 1;


    public void Generate()
    {
        Random.InitState(seed);

        var gravityBodies = new GravityBody[planetCountRange.Random()];
        var planetSystem = Utils.SpawnPrefab("Planet System").GetComponent<PlanetSystem>();
        planetSystem.transform.position = center;
        var systemName = GenerateSystemName(planetSystem.gameObject);

        for (var i = 0; i < gravityBodies.Length; i++)
        {
            gravityBodies[i] = planetGenerator.Generate();
        }

        var orderedBodies = gravityBodies.OrderBy(body => body.radius * body.Mass).ToList();

        var sun = sunGenerator.Generate();
        var sunTransform = sun.transform;
        sun.bodyName = $"{systemName} Sun";
        sunTransform.parent = planetSystem.transform;
        planetSystem.bodies = gravityBodies.Append(sun).ToArray();

        float nextOrbit;
        foreach (var gravityBody in orderedBodies)
        {
            nextOrbit = orbitDistanceRadius.Random() + sunGenerator.planetRadiusRange.to;
            //sqrt(G*(m1 + m2)/ r)
            var bodyTransform = gravityBody.transform;
            bodyTransform.parent = planetSystem.transform;

            var onOrbitPosition = Random.onUnitSphere * nextOrbit;
            bodyTransform.position = onOrbitPosition;
            gravityBody.orbitRadius = onOrbitPosition.magnitude;
            gravityBody.bodyName = $"{systemName} {_alphabetLower[Random.Range(0, _alphabetLower.Length)]}";

            var centralBodyDirection = (sunTransform.position - bodyTransform.position).normalized;
            var left = Vector3.Cross(centralBodyDirection, sunTransform.up);
            gravityBody.initialVelocity = left.normalized
                                          * (1.05f
                                             * Mathf.Sqrt(
                                                 Universe.GravitationConstant
                                                 * (gravityBody.Mass + sun.Mass)
                                                 / gravityBody.orbitRadius));
        }
    }

    private string GenerateSystemName(GameObject system)
    {
        var systemObject = system.gameObject;

        var systemName = "";
        for (int i = 0, length = nameLengthRange.Random(); i < length; i++)
        {
            systemName += _alphabet[Random.Range(0, _alphabet.Length)];
        }

        systemName += "-";
        for (int i = 0, length = nameLengthRange.Random(); i < length; i++)
        {
            systemName += _digits[Random.Range(0, _digits.Length)];
        }

        return systemObject.name = systemName;
    }
}