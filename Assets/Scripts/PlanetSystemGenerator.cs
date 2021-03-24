using System.Linq;
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


    public void Generate()
    {
        planetGenerator.SetupSeed();
        var gravityBodies = new GravityBody[planetCountRange.Random()];
        var planetSystem = Utils.SpawnPrefab("Planet System").GetComponent<PlanetSystem>();
        planetSystem.transform.position = center;
        planetSystem.bodies = gravityBodies;
        var systemName = GenerateSystemName(planetSystem.gameObject);

        for (var i = 0; i < gravityBodies.Length; i++)
        {
            gravityBodies[i] = planetGenerator.Generate();
        }

        var orderedBodies = gravityBodies.OrderByDescending(body => body.radius * body.Mass).ToList();
        var sun = orderedBodies[0];
        var sunTransform = sun.transform;

        sun.bodyName = $"{systemName} Sun";
        sun.GetComponent<Rigidbody>().mass *= 10;
        sunTransform.parent = planetSystem.transform;

        var nextOrbit = orbitDistanceRadius.Random() + planetGenerator.planetRadiusRange.to;
        foreach (var gravityBody in orderedBodies.Skip(1).Reverse())
        {
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
                                          * (1.15f
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