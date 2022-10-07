using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Planet_System;
using Planet.Common;
using Planet.Settings.Generation;
using UnityEngine;
using Utils;
using Debug = UnityEngine.Debug;

namespace Planet.Generation_Methods.Multithreaded.PlanetSystemGenerators
{
    public class PlanetSystemGeneratorAsync : BasePlanetSystemGenerator
    {
        [Space]
        [SerializeField] protected PlanetParametersGeneratorAsync planetParametersGenerator;
        [SerializeField] protected SunParametersGeneratorAsync sunParametersGenerator;


        public override void SetGenerationParameters(PlanetGenerationParameters planetParameters,
            SunGenerationParameters sunParameters, PlanetSystemGenerationParameters planetSystemParameters, int seed)
        {
            planetParametersGenerator.parameters = planetParameters;
            sunParametersGenerator.parameters = sunParameters;
            parameters = planetSystemParameters;
            this.seed = seed;
        }

        public override async void Generate(int seed)
        {
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();

            Time.timeScale = 0;
            Random.InitState(seed);

            var planetSystem = SpawnUtils.SpawnPrefab(planetSystemPrefab).GetComponent<BasePlanetSystem>();
            planetSystem.enabled = false;
            var systemName = GenerateSystemName(planetSystem.gameObject);
            var planetSystemTransform = planetSystem.transform;
            planetSystemTransform.position = parameters.center;

            var mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var gravityBodyTasks = ParallelEnumerable.Range(1, parameters.planetCountRange.RandomValue)
                .Select(i => planetParametersGenerator.Generate(seed - i, mainTaskScheduler))
                .ToArray();

            var sun = await GenerateSun(seed, systemName, planetSystemTransform, mainTaskScheduler);
            sun.enabled = true;

            await Task.WhenAll(gravityBodyTasks);
            var orderedBodies = gravityBodyTasks.Select(bodyTask => bodyTask.Result)
                .OrderBy(body => body.radius * body.Mass)
                .ToList();

            PlaceBodiesOnOrbits(orderedBodies, planetSystemTransform, sun,
                sunParametersGenerator.parameters.planetRadiusRange.to, systemName);

            orderedBodies.Add(sun);
            planetSystem.Bodies = orderedBodies.ToArray();
            planetSystem.enabled = true;

            Time.timeScale = 1;

            stopwatch.Stop();
            Debug.Log($"Generated in {stopwatch.Elapsed}");
        }

        private async Task<GravityBody> GenerateSun(int sunSeed, string systemName, Transform planetSystemTransform,
            TaskScheduler main)
        {
            var sun = await Task.Run(() => sunParametersGenerator.Generate(sunSeed, main));
            // var sun = sunTask.Result;
            sun.bodyName = $"{systemName} Sun";
            sun.transform.parent = planetSystemTransform;
            SetDefaultTargetForCamera(sun.gameObject);
            return sun;
        }
    }
}
