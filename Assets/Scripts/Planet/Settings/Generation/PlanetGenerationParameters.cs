using Planet.Common;
using UnityEngine;

namespace Planet.Settings.Generation
{
    [CreateAssetMenu(menuName = "Generation/Planet Parameters", order = 0)]
    public class PlanetGenerationParameters : BasePlanetGenerationParameters
    {
        [Space]
        [Header("Biomes")]
        [SerializeField] public IntRange biomesRange;

        [SerializeField] public IntRange biomeColorCountRange;
        [SerializeField] public FloatRange biomeNoiseOffsetRange;
        [SerializeField] public FloatRange biomeStrengthRange;
        [SerializeField] public FloatRange biomeBlendRange;
        [SerializeField] public IntRange biomeOceanColorCountRange;
    }
}
