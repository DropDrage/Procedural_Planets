using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PlanetSystemGenerator))]
    public class PlanetSystemGeneratorEditor : UnityEditor.Editor
    {
        private PlanetSystemGenerator generator;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate"))
            {
                generator.Generate();
            }
        }


        private void OnEnable()
        {
            generator = target as PlanetSystemGenerator;
        }
    }
}