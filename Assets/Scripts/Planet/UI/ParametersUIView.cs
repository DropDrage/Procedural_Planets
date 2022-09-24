using System;
using Planet.Generation_Async.PlanetSystemGenerators;
using Planet.UI.Tab;
using UI.Tabs_View;
using UnityEngine;
using UnityEngine.UIElements;

namespace Planet.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class ParametersUIView : MonoBehaviour
    {
        [SerializeField] private PlanetSystemGeneratorAsync planetSystemGenerator;

        private bool _isTabContentVisible = true;

        private TabsView _tabsView;

        private PlanetGenerationParametersTab _planetGenerationParametersTab;
        private SunGenerationParametersTab _sunGenerationParametersTab;
        private PlanetSystemGenerationParametersTab _planetSystemGenerationParametersTab;

        private Button _hideButton;
        private Button _generateButton;


        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _hideButton = root.Q<Button>("HideButton");
            _hideButton.clicked += HidePanel;

            _tabsView = root.Q<TabsView>("TabsView");
            _tabsView.TabContentVisibilityChanged += OnTabContentVisibilityChanged;
            var tabs = _tabsView.Tabs;
            _planetGenerationParametersTab = (PlanetGenerationParametersTab) Array.Find(tabs,
                element => element is PlanetGenerationParametersTab);
            _sunGenerationParametersTab = (SunGenerationParametersTab) Array.Find(tabs,
                element => element is SunGenerationParametersTab);
            _planetSystemGenerationParametersTab = (PlanetSystemGenerationParametersTab) Array.Find(tabs,
                element => element is PlanetSystemGenerationParametersTab);

            _generateButton = root.Q<Button>("GenerateButton");
            _generateButton.clicked += GeneratePlanetSystem;
        }

        private void OnTabContentVisibilityChanged(bool isVisible)
        {
            _isTabContentVisible = isVisible;
        }

        private void HidePanel()
        {
            _tabsView.IsTabContentVisible = _isTabContentVisible = !_isTabContentVisible;
        }

        private void GeneratePlanetSystem()
        {
            _generateButton.SetEnabled(false);

            planetSystemGenerator.SetGenerationParameters(
                _planetGenerationParametersTab.Parameters, _sunGenerationParametersTab.Parameters,
                _planetSystemGenerationParametersTab.Parameters,
                _planetSystemGenerationParametersTab.Seed);
            planetSystemGenerator.Generate();

            _generateButton.SetEnabled(true);
        }

        private void OnDestroy()
        {
            _tabsView.TabContentVisibilityChanged -= OnTabContentVisibilityChanged;

            _generateButton.clicked -= GeneratePlanetSystem;
            _hideButton.clicked -= HidePanel;
        }
    }
}
