using Planet.Settings.Generation;
using Planet.Utils;
using UnityEngine.UIElements;

namespace Planet.UI.Tab
{
    public class SunGenerationParametersTab : BasePlanetGenerationParametersTab<SunGenerationParameters>
    {
        public new class UxmlFactory : UxmlFactory<SunGenerationParametersTab, UxmlTraits>
        {
        }

        private const int LightMultiplier = 100000;

        protected override string UxmlName => "Sun Parameters Layout";

        public override SunGenerationParameters Parameters
        {
            get
            {
                parameters = base.Parameters;

                // Biomes
                parameters.biomeOceanColorCountRange = _biomeOceanColorCountRange.ToIntRange();
                parameters.lightIntensityRange = _lightIntensityRange.ToIntRange(LightMultiplier);
                return parameters;
            }
        }

        private MinMaxSlider _biomeOceanColorCountRange;
        private MinMaxSlider _lightIntensityRange;


        protected override void ObtainViews()
        {
            base.ObtainViews();

            _biomeOceanColorCountRange = this.Q<MinMaxSlider>("OceanColorCount");
            _lightIntensityRange = this.Q<MinMaxSlider>("LightIntensity");
        }
    }
}
