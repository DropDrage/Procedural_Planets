using System;
using System.Diagnostics;
using System.Linq;
using Planet.Common;
using Planet.Settings.Generation;
using UnityEngine;
using Utils;
using Debug = UnityEngine.Debug;
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
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();

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

            var sun = GenerateSun(systemName, planetSystemTransform);

            var orderedBodies = gravityBodies.OrderBy(body => body.radius * body.Mass);
            PlaceBodiesOnOrbits(orderedBodies, planetSystemTransform, sun,
                sunParametersGenerator.parameters.planetRadiusRange.to, systemName);

            // gravityBodies.Add(sun);
            planetSystem.bodies = gravityBodies.Append(sun).ToArray();

            stopwatch.Stop();
            Debug.Log($"Generated in {stopwatch.Elapsed}");
        }

        private GravityBody GenerateSun(string systemName, Transform planetSystemTransform)
        {
            var sun = sunParametersGenerator.Generate();
            sun.bodyName = $"{systemName} Sun";
            sun.transform.parent = planetSystemTransform;
            SetDefaultTargetForCamera(sun.gameObject);
            return sun;
        }
    }
}
