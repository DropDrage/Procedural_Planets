using Planet.Common;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(BasePlanetSystemGenerator), true)]
    public class PlanetSystemGeneratorEditor : UnityEditor.Editor
    {
        private BasePlanetSystemGenerator _generator;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate"))
            {
                _generator.Generate();
            }
        }


        private void OnEnable()
        {
            _generator = target as BasePlanetSystemGenerator;
        }
    }
}
