using Planet.Settings.Generation;
using Planet.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Planet.UI.Tab
{
    public class PlanetSystemGenerationParametersTab : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<PlanetSystemGenerationParametersTab, UxmlTraits>
        {
        }

        private const float OrbitDistanceMultiplier = 100;

        private const string UxmlName = "Planet System Parameters Layout";

        protected PlanetSystemGenerationParameters parameters;

        public PlanetSystemGenerationParameters Parameters
        {
            get
            {
                parameters.nameLengthRange = _nameLengthRange.ToIntRange();
                parameters.orbitDistanceRadius = _orbitDistanceRadius.ToFloatRange(OrbitDistanceMultiplier);
                parameters.planetCountRange = _planetCountRange.ToIntRange();

                return parameters;
            }
        }

        public int Seed => _seed.value;

        private MinMaxSlider _nameLengthRange;
        private MinMaxSlider _orbitDistanceRadius;
        private MinMaxSlider _planetCountRange;

        private SliderInt _seed;


        public PlanetSystemGenerationParametersTab()
        {
            Init();
        }

        private void Init()
        {
            parameters = ScriptableObject.CreateInstance<PlanetSystemGenerationParameters>();

            var visualTree = Resources.Load<VisualTreeAsset>($"UXML/{UxmlName}");
            visualTree.CloneTree(this);

            ObtainViews();
        }

        private void ObtainViews()
        {
            _nameLengthRange = this.Q<MinMaxSlider>("NameLength");
            _orbitDistanceRadius = this.Q<MinMaxSlider>("OrbitDistanceRadius");
            _planetCountRange = this.Q<MinMaxSlider>("PlanetCount");

            _seed = this.Q<SliderInt>("Seed");
        }
    }
}
