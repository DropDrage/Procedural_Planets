using Planet.Common;
using UnityEngine;

namespace Planet.Settings.Generation
{
    [CreateAssetMenu(menuName = "Generation/Sun Parameters", order = 1)]
    public class SunGenerationParameters : BasePlanetGenerationParameters
    {
        [Space]
        [SerializeField] public IntRange biomeOceanColorCountRange;
        [SerializeField] public IntRange lightIntensityRange;
    }
}
