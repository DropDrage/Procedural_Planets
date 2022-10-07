using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Planet_System;
using Planet.Common;
using Planet.Settings.Generation;
using Unity.Jobs;
using UnityEngine;
using Utils;
using Debug = UnityEngine.Debug;

namespace Planet.Generation_Methods.Jobs.PlanetSystemGenerators
{
    public class PlanetSystemGeneratorJobs : BasePlanetSystemGenerator
    {
        [Space]
        [SerializeField] protected PlanetParametersGeneratorJob planetParametersGenerator;
        [SerializeField] protected SunParametersGeneratorJob sunParametersGenerator;


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

            var planetsCount = parameters.planetCountRange.RandomValue;
            var gravityBodies = new GravityBody[planetsCount];
            var mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var planetGenerationJob = new PlanetGenerationJob(
                mainTaskScheduler,
                planetParametersGenerator,
                gravityBodies,
                seed
            );
            var planetGenerationJobHandle = planetGenerationJob.Schedule(planetsCount, 0);

            var sun = await GenerateSun(seed, systemName, planetSystemTransform, mainTaskScheduler);
            sun.enabled = true;

            planetGenerationJobHandle.Complete();
            var orderedBodies = gravityBodies
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
