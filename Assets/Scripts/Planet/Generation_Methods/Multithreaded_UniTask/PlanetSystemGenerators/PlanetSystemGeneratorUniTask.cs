using System.Diagnostics;
using System.Linq;
using Cysharp.Threading.Tasks;
using Planet.Common;
using Planet.Settings.Generation;
using Planet_System;
using UnityEngine;
using Utils;
using Debug = UnityEngine.Debug;

namespace Planet.Generation_Methods.Multithreaded_UniTask.PlanetSystemGenerators
{
    public class PlanetSystemGeneratorUniTask : BasePlanetSystemGenerator
    {
        [Space]
        [SerializeField] protected PlanetParametersGeneratorUniTask planetParametersGenerator;
        [SerializeField] protected SunParametersGeneratorUniTask sunParametersGenerator;


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

            await UniTask.SwitchToThreadPool();

            var planetsBodiesTasks = new UniTask<GravityBody>[planetsCount];
            for (var i = 1; i <= planetsCount; i++)
            {
                planetsBodiesTasks[i] = planetParametersGenerator.Generate(seed - i);
            }

            var sun = await GenerateSun(seed, systemName, planetSystemTransform);

            var planetsBodies = await UniTask.WhenAll(planetsBodiesTasks);
            await UniTask.SwitchToMainThread();
            var orderedBodies = planetsBodies.OrderBy(body => body.radius * body.Mass).ToList();

            PlaceBodiesOnOrbits(
                orderedBodies, planetSystemTransform, sun,
                sunParametersGenerator.parameters.planetRadiusRange.to, systemName
            );

            orderedBodies.Add(sun);
            planetSystem.Bodies = orderedBodies.ToArray();
            planetSystem.enabled = true;

            Time.timeScale = 1;

            stopwatch.Stop();
            Debug.Log($"Generated in {stopwatch.Elapsed}");
        }

        private async UniTask<GravityBody> GenerateSun(int sunSeed, string systemName, Transform planetSystemTransform)
        {
            var sun = await sunParametersGenerator.Generate(sunSeed);
            await UniTaskUtils.RunOnMainThreadFromThreadPool(
                sun =>
                {
                    sun.bodyName = $"{systemName} Sun";
                    sun.transform.parent = planetSystemTransform;
                    sun.enabled = true;
                    SetDefaultTargetForCamera(sun.gameObject);
                }, sun
            );
            return sun;
        }
    }
}
