using Planet.Settings.Generation;
using Planet.Utils;
using UnityEngine.UIElements;

namespace Planet.UI.Tab
{
    public class PlanetGenerationParametersTab : BasePlanetGenerationParametersTab<PlanetGenerationParameters>
    {
        public new class UxmlFactory : UxmlFactory<PlanetGenerationParametersTab, UxmlTraits>
        {
        }

        protected override string UxmlName => "Planet Parameters Layout";

        public override PlanetGenerationParameters Parameters
        {
            get
            {
                parameters = base.Parameters;

                // Biomes
                parameters.biomesRange = _biomeCountRange.ToIntRange();
                parameters.biomeColorCountRange = _biomeColorCountRange.ToIntRange();
                parameters.biomeNoiseOffsetRange = _biomeNoiseOffsetRange.ToFloatRange();
                parameters.biomeStrengthRange = _biomeStrengthRange.ToFloatRange();
                parameters.biomeBlendRange = _biomeBlendRange.ToFloatRange();
                parameters.biomeOceanColorCountRange = _biomeOceanColorCountRange.ToIntRange();
                return parameters;
            }
        }

        #region Biomes

        private MinMaxSlider _biomeCountRange;
        private MinMaxSlider _biomeColorCountRange;
        private MinMaxSlider _biomeNoiseOffsetRange;
        private MinMaxSlider _biomeStrengthRange;
        private MinMaxSlider _biomeBlendRange;
        private MinMaxSlider _biomeOceanColorCountRange;

        #endregion


        protected override void ObtainViews()
        {
            base.ObtainViews();

            // Biomes
            _biomeCountRange = this.Q<MinMaxSlider>("BiomeCount");
            _biomeColorCountRange = this.Q<MinMaxSlider>("BiomeColorCount");
            _biomeNoiseOffsetRange = this.Q<MinMaxSlider>("BiomeNoiseOffset");
            _biomeStrengthRange = this.Q<MinMaxSlider>("BiomeStrength");
            _biomeBlendRange = this.Q<MinMaxSlider>("BiomeBlend");
            _biomeOceanColorCountRange = this.Q<MinMaxSlider>("BiomeOceanColorCount");
        }
    }
}
