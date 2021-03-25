using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PlanetSystemGenerator))]
    public class PlanetSystemGeneratorEditor : UnityEditor.Editor
    {
        private PlanetSystemGenerator _generator;

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
            _generator = target as PlanetSystemGenerator;
        }
    }
}