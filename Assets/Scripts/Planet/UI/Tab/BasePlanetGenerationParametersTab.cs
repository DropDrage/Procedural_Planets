using Planet.Settings.Generation;
using Planet.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Planet.UI.Tab
{
    public abstract class BasePlanetGenerationParametersTab<T> : VisualElement where T : BasePlanetGenerationParameters
    {
        protected abstract string UxmlName { get; }

        protected T parameters;

        public virtual T Parameters
        {
            get
            {
                parameters.planetRadiusRange = _planetRadiusRange.ToFloatRange();
                parameters.noiseLayersRange = _noiseLayersRange.ToIntRange();

                // Noise
                parameters.layersInNoiseCountRange = _layersInNoiseRange.ToIntRange();
                parameters.baseRoughnessRange = _baseRoughnessRange.ToFloatRange();
                parameters.roughnessRange = _roughnessRange.ToFloatRange();
                parameters.persistenceRange = _persistenceRange.ToFloatRange();
                parameters.centerMagnitudeRange = _centerMagnitudeRange.ToFloatRange();
                parameters.weightRange = _weightRange.ToFloatRange();
                parameters.strengthRange = _strengthRange.ToFloatRange();

                // Gravity
                parameters.angularVelocityRange = _angularVelocityRange.ToFloatRange();
                parameters.massMultiplierRange = _massMultiplierRange.ToFloatRange();

                return parameters;
            }
        }

        private MinMaxSlider _planetRadiusRange;
        private MinMaxSlider _noiseLayersRange;

        #region Noise

        private MinMaxSlider _layersInNoiseRange;
        private MinMaxSlider _baseRoughnessRange;
        private MinMaxSlider _roughnessRange;
        private MinMaxSlider _persistenceRange;
        private MinMaxSlider _centerMagnitudeRange;
        private MinMaxSlider _weightRange;
        private MinMaxSlider _strengthRange;

        #endregion

        #region Gravity

        private MinMaxSlider _angularVelocityRange;
        private MinMaxSlider _massMultiplierRange;

        #endregion


        public BasePlanetGenerationParametersTab()
        {
            Init();
        }

        private void Init()
        {
            parameters = ScriptableObject.CreateInstance<T>();

            var visualTree = Resources.Load<VisualTreeAsset>($"UXML/{UxmlName}");
            visualTree.CloneTree(this);

            ObtainViews();
        }

        protected virtual void ObtainViews()
        {
            _planetRadiusRange = this.Q<MinMaxSlider>("PlanetRadius");
            _noiseLayersRange = this.Q<MinMaxSlider>("NoiseLayersCount");

            // Noise
            _layersInNoiseRange = this.Q<MinMaxSlider>("LayersInNoiseCount");
            _baseRoughnessRange = this.Q<MinMaxSlider>("BaseRoughness");
            _roughnessRange = this.Q<MinMaxSlider>("Roughness");
            _persistenceRange = this.Q<MinMaxSlider>("Persistence");
            _centerMagnitudeRange = this.Q<MinMaxSlider>("CenterMagnitude");
            _weightRange = this.Q<MinMaxSlider>("Weight");
            _strengthRange = this.Q<MinMaxSlider>("Strength");

            // Gravity
            _angularVelocityRange = this.Q<MinMaxSlider>("AngularVelocity");
            _massMultiplierRange = this.Q<MinMaxSlider>("MassMultiplier");
        }
    }
}
