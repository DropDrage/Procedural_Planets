using System;
using System.Linq;
using Planet.Common;
using Planet.Settings.Generation;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Planet.Generation_Methods.Singlethreaded.PlanetSystemGenerators
{
    [Obsolete("Use async")]
    public class PlanetSystemGenerator : BasePlanetSystemGenerator
    {
        [SerializeField] private bool withSun = true;

        [SerializeField] protected PlanetParametersGenerator planetParametersGenerator;
        [SerializeField] protected SunParametersGenerator sunParametersGenerator;


        public override void SetGenerationParameters(PlanetGenerationParameters planetParameters,
            SunGenerationParameters sunParameters,
            PlanetSystemGenerationParameters planetSystemParameters, int seed)
        {
            planetParametersGenerator.parameters = planetParameters;
            sunParametersGenerator.parameters = sunParameters;
            parameters = planetSystemParameters;
            this.seed = seed;
        }

        public override void Generate(int seed)
        {
            Random.InitState(seed);

            var planetSystem = SpawnUtils.SpawnPrefab(planetSystemPrefab).GetComponent<PlanetSystem>();
            planetSystem.gameObject.SetActive(true);
            var planetSystemTransform = planetSystem.transform;
            planetSystemTransform.position = parameters.center;

            var gravityBodies = new GravityBody[parameters.planetCountRange.RandomValue];
            for (var i = 0; i < gravityBodies.Length; i++)
            {
                gravityBodies[i] = planetParametersGenerator.Generate();
            }

            var systemName = GenerateSystemName(planetSystem.gameObject);
            if (!withSun)
            {
                return;
            }

            var sun = GenerateSun(systemName, planetSystemTransform, out var sunTransform);
            var sunPosition = sunTransform.position;

            var orderedBodies = gravityBodies.OrderBy(body => body.radius * body.Mass).ToArray();
            foreach (var gravityBody in orderedBodies)
            {
                var nextOrbit = parameters.orbitDistanceRadius.RandomValue +
                                sunParametersGenerator.parameters.planetRadiusRange.to;
                //sqrt(G*(m1 + m2)/ r)
                var bodyTransform = gravityBody.transform;
                bodyTransform.parent = planetSystemTransform;

                var onOrbitPosition = Random.onUnitSphere * nextOrbit;
                bodyTransform.position = onOrbitPosition;
                gravityBody.orbitRadius = onOrbitPosition.magnitude;
                gravityBody.bodyName =
                    $"{systemName} {alphabetLower[Random.Range(0, alphabetLower.Length)]}"; //ToDo no random?

                var sunDirection = (sunPosition - bodyTransform.position).normalized;
                var left = Vector3.Cross(sunDirection, sunTransform.up);
                gravityBody.initialVelocity = left.normalized * (1.05f * Mathf.Sqrt(
                    Universe.GravitationConstant
                    * (gravityBody.Mass + sun.Mass)
                    / gravityBody.orbitRadius));
            }

            planetSystem.bodies = gravityBodies.Append(sun).ToArray();
        }

        private GravityBody GenerateSun(string systemName, Transform planetSystemTransform,
            out Transform sunTransform)
        {
            var sun = sunParametersGenerator.Generate();
            sunTransform = sun.transform;
            sun.bodyName = $"{systemName} Sun";
            sunTransform.parent = planetSystemTransform;
            SetDefaultTargetForCamera(sun.gameObject);
            return sun;
        }
    }
}
