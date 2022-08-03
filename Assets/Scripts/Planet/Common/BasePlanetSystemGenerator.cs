using System.Text;
using UnityEngine;
using Utils;

namespace Planet.Common
{
    public abstract class BasePlanetSystemGenerator : MonoBehaviour
    {
        protected readonly char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        protected readonly char[] alphabetLower = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray();
        protected readonly char[] digits = "1234567890".ToCharArray();

        [SerializeField] protected GameObject planetSystemPrefab;

        [Space]
        [SerializeField] protected IntRange nameLengthRange;

        [SerializeField] protected FloatRange orbitDistanceRadius;
        [SerializeField] protected IntRange planetCountRange;

        [SerializeField] protected Vector3 center;

        [Space]
        [Range(0, int.MaxValue), SerializeField]
        protected int seed = 1;


        public abstract void Generate();


        protected string GenerateSystemName(GameObject system)
        {
            var systemName = new StringBuilder();
            for (int i = 0, length = nameLengthRange.RandomValue; i < length; i++)
            {
                systemName.Append(alphabet[Random.Range(0, alphabet.Length)]);
            }

            systemName.Append('-');
            for (int i = 0, length = nameLengthRange.RandomValue; i < length; i++)
            {
                systemName.Append(digits[Random.Range(0, digits.Length)]);
            }

            return system.name = systemName.ToString();
        }

        protected static void SetDefaultTargetForCamera(GameObject target)
        {
            var mainOrbitCamera = FindObjectOfType<OrbitCamera>();
            if (mainOrbitCamera != null && !mainOrbitCamera.HasTarget)
            {
                mainOrbitCamera.Target = target;
            }
        }
    }
}
