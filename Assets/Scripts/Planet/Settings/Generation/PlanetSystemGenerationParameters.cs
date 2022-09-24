using Planet.Common;
using UnityEngine;

namespace Planet.Settings.Generation
{
    [CreateAssetMenu(menuName = "Generation/Planet System Parameters", order = 2)]
    public class PlanetSystemGenerationParameters : ScriptableObject
    {
        [SerializeField] public IntRange nameLengthRange;

        [SerializeField] public FloatRange orbitDistanceRadius;
        [SerializeField] public IntRange planetCountRange;

        [SerializeField] public Vector3 center;
    }
}
