using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlanetSystemGenerators
{
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

            var gravityBodies = new GravityBody[planetCountRange.RandomValue];
            var planetSystem = Utils.SpawnPrefab("Planet System").GetComponent<PlanetSystem>();
            planetSystem.transform.position = center;
            var systemName = GenerateSystemName(planetSystem.gameObject);

            for (var i = 0; i < gravityBodies.Length; i++)
            {
                gravityBodies[i] = planetGenerator.Generate();
            }

            var orderedBodies = gravityBodies.OrderBy(body => body.radius * body.Mass).ToList();
            var sun = GenerateSun(systemName, planetSystem, gravityBodies, out var sunTransform);

            foreach (var gravityBody in orderedBodies)
            {
                var nextOrbit = orbitDistanceRadius.RandomValue + sunGenerator.planetRadiusRange.to;
                //sqrt(G*(m1 + m2)/ r)
                var bodyTransform = gravityBody.transform;
                bodyTransform.parent = planetSystem.transform;

                Vector3 onOrbitPosition = Random.onUnitSphere * nextOrbit;
                bodyTransform.position = onOrbitPosition;
                gravityBody.orbitRadius = onOrbitPosition.magnitude;
                gravityBody.bodyName = $"{systemName} {_alphabetLower[Random.Range(0, _alphabetLower.Length)]}";

                var sunDirection = (sunTransform.position - bodyTransform.position).normalized;
                Vector3 left = Vector3.Cross(sunDirection, sunTransform.up);
                gravityBody.initialVelocity = left.normalized
                                              * (1.05f
                                                 * Mathf.Sqrt(
                                                     Universe.GravitationConstant
                                                     * (gravityBody.Mass + sun.Mass)
                                                     / gravityBody.orbitRadius));
            }
        }

        private GravityBody GenerateSun(string systemName, PlanetSystem planetSystem,
                                        IEnumerable<GravityBody> gravityBodies,
                                        out Transform sunTransform)
        {
            var sun = sunGenerator.Generate();
            sunTransform = sun.transform;
            sun.bodyName = $"{systemName} Sun";
            sunTransform.parent = planetSystem.transform;
            planetSystem.bodies = gravityBodies.Append(sun).ToArray();
            SetDefaultTargetForCamera(sun.gameObject);
            return sun;
        }

        private string GenerateSystemName(GameObject system)
        {
            var systemObject = system.gameObject;

            var systemName = "";
            for (int i = 0, length = nameLengthRange.RandomValue; i < length; i++)
            {
                systemName += _alphabet[Random.Range(0, _alphabet.Length)];
            }

            systemName += "-";
            for (int i = 0, length = nameLengthRange.RandomValue; i < length; i++)
            {
                systemName += _digits[Random.Range(0, _digits.Length)];
            }

            return systemObject.name = systemName;
        }

        private static void SetDefaultTargetForCamera(GameObject target)
        {
            var mainOrbitCamera = FindObjectOfType<OrbitCamera>();
            if (mainOrbitCamera != null && !mainOrbitCamera.HasTarget)
            {
                mainOrbitCamera.Target = target;
            }
        }
    }
}