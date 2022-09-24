using Camera_Controls;
using Planet.Settings.Generation;
using UnityEngine;
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
