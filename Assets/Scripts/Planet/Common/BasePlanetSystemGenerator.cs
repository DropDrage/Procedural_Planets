using System.Collections.Generic;
using Camera_Controls;
using Planet.Settings.Generation;
using UnityEngine;
using Utils;
using Utils.Extensions;

namespace Planet.Common
{
    public abstract class BasePlanetSystemGenerator : MonoBehaviour
    {
        protected readonly char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        protected readonly char[] alphabetLower = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray();
        protected readonly char[] digits = "1234567890".ToCharArray();

        [SerializeField] protected GameObject planetSystemPrefab;

        [SerializeField] protected PlanetSystemGenerationParameters parameters;

        [Space]
        [Range(0, int.MaxValue), SerializeField]
        protected int seed = 1;


        public abstract void SetGenerationParameters(PlanetGenerationParameters planetParameters,
            SunGenerationParameters sunParameters, PlanetSystemGenerationParameters planetSystemParameters,
            int seed);

        public void Generate()
        {
            Generate(seed);
        }

        public abstract void Generate(int seed);


        protected string GenerateSystemName(GameObject system)
        {
            var lettersCount = parameters.nameLengthRange.RandomValue;
            var digitsCount = parameters.nameLengthRange.RandomValue;
            return system.name = string.Create(lettersCount + 1 + digitsCount, lettersCount, (span, nameLength) =>
            {
                for (var i = 0; i < nameLength; i++)
                {
                    span[i] = alphabet.GetRandomItem();
                }

                span[nameLength] = '-';

                for (int i = nameLength + 1, length = span.Length; i < length; i++)
                {
                    span[i] = digits.GetRandomItem();
                }
            });
        }

        protected void PlaceBodiesOnOrbits(IEnumerable<GravityBody> orderedBodies, Transform planetSystemTransform,
            GravityBody sun, float maxSunRadius, string systemName)
        {
            var sunTransform = sun.transform;
            var sunPosition = sunTransform.position;
            foreach (var gravityBody in orderedBodies)
            {
                var nextOrbit = parameters.orbitDistanceRadius.RandomValue + maxSunRadius;
                var bodyTransform = gravityBody.transform;
                bodyTransform.SetParent(planetSystemTransform);

                var onOrbitPosition = Random.onUnitSphere * nextOrbit;
                bodyTransform.position = onOrbitPosition;
                var orbitRadius = onOrbitPosition.magnitude;
                //ToDo no random?
                gravityBody.bodyName = $"{systemName} {alphabetLower[Random.Range(0, alphabetLower.Length)]}";

                var directionToSun = sunPosition - bodyTransform.position;
                var left = Vector3.Cross(directionToSun, sunTransform.up);
                //sqrt(G*(m1 + m2)/ r)
                gravityBody.initialVelocity = left.normalized * (1.05f
                        * Mathf.Sqrt(
                            Universe.GravitationConstant * (gravityBody.Mass + sun.Mass)
                            / orbitRadius)
                    );

                gravityBody.Initialize();
            }
        }


        protected static void SetDefaultTargetForCamera(GameObject target)
        {
            var mainOrbitCamera = FindObjectOfType<OrbitCamera>(); //ToDo serializedField
            if (mainOrbitCamera != null && !mainOrbitCamera.HasTarget)
            {
                mainOrbitCamera.Target = target;
            }
        }
    }
}
