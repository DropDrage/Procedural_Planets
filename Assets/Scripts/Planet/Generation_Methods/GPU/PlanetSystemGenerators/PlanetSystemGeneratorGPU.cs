using System.Linq;
using Planet.Common;
using Planet.Settings.Generation;
using UnityEngine;
using Utils;

namespace Planet.Generation_Methods.GPU.PlanetSystemGenerators
{
    public class PlanetSystemGeneratorGPU : BasePlanetSystemGenerator
    {
        [Space]
        [SerializeField] protected PlanetParametersGeneratorGPU planetParametersGenerator;
        [SerializeField] protected SunParametersGeneratorGPU sunParametersGenerator;


        public override void SetGenerationParameters(PlanetGenerationParameters planetParameters,
            SunGenerationParameters sunParameters, PlanetSystemGenerationParameters planetSystemParameters, int seed)
        {
            planetParametersGenerator.parameters = planetParameters;
            sunParametersGenerator.parameters = sunParameters;
            parameters = planetSystemParameters;
            this.seed = seed;
        }

        public override void Generate(int seed)
        {
            Time.timeScale = 0;
            Debug.Log("Generation start");
            Random.InitState(seed);

            var planetSystem = SpawnUtils.SpawnPrefab(planetSystemPrefab).GetComponent<PlanetSystem>();
            planetSystem.enabled = false;
            var systemName = GenerateSystemName(planetSystem.gameObject);
            var planetSystemTransform = planetSystem.transform;
            planetSystemTransform.position = parameters.center;

            var gravityBodies = new GravityBody[parameters.planetCountRange.RandomValue];
            for (var i = 0; i < gravityBodies.Length; i++)
            {
                gravityBodies[i] = planetParametersGenerator.Generate(seed - i);
            }

            var sun = GenerateSun(seed, systemName, planetSystemTransform);
            sun.enabled = true;
            var sunTransform = sun.transform;
            var sunPosition = sunTransform.position;

            var orderedBodies = gravityBodies.OrderBy(body => body.radius * body.Mass)
                .ToArray();

            foreach (var gravityBody in orderedBodies)
            {
                var nextOrbit = parameters.orbitDistanceRadius.RandomValue +
                                sunParametersGenerator.parameters.planetRadiusRange.to;
                var bodyTransform = gravityBody.transform;
                bodyTransform.parent = planetSystemTransform;

                var onOrbitPosition = Random.onUnitSphere * nextOrbit;
                bodyTransform.position = onOrbitPosition;
                gravityBody.orbitRadius = onOrbitPosition.magnitude;
                gravityBody.bodyName = $"{systemName} {alphabetLower[Random.Range(0, alphabetLower.Length)]}";

                var sunDirection = (sunPosition - bodyTransform.position).normalized;
                var left = Vector3.Cross(sunDirection, sunTransform.up);
                //sqrt(G*(m1 + m2)/ r)
                gravityBody.initialVelocity = left.normalized * (1.05f * Mathf.Sqrt(
                    Universe.GravitationConstant * (gravityBody.Mass + sun.Mass) / gravityBody.orbitRadius));
                gravityBody.enabled = true;
            }

            planetSystem.enabled = true;
            planetSystem.bodies = orderedBodies.Append(sun).ToArray();

            Debug.Log("Generation end");
            Time.timeScale = 1;
        }

        private GravityBody GenerateSun(int sunSeed, string systemName, Transform planetSystemTransform)
        {
            var sun = sunParametersGenerator.Generate(sunSeed);
            // var sun = sunTask.Result;
            sun.bodyName = $"{systemName} Sun";
            sun.transform.parent = planetSystemTransform;
            SetDefaultTargetForCamera(sun.gameObject);
            return sun;
        }
    }
}
