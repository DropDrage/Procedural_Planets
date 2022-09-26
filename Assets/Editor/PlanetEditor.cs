using System;
using Planet.Generation_Methods.Singlethreaded;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    [CustomEditor(typeof(PlanetAutoGenerator), true)]
    public class PlanetEditor : UnityEditor.Editor
    {
        private PlanetAutoGenerator _planetGenerator;

        private UnityEditor.Editor _shapeEditor;
        private UnityEditor.Editor _colourEditor;

        [SerializeField] private bool autoUpdate;

        private static bool _shapeFoldout;
        private static bool _colorFoldout;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            autoUpdate = false;

            GUILayout.Space(20);

            DrawSettingsEditor(_planetGenerator.shapeSettings, _planetGenerator.OnShapeSettingsUpdated,
                ref _shapeFoldout,
                ref _shapeEditor);
            DrawSettingsEditor(_planetGenerator.colorSettings, _planetGenerator.OnColorSettingsUpdated,
                ref _colorFoldout,
                ref _colourEditor);

            GUILayout.Space(20);

            if (GUILayout.Button("Generate Planet"))
            {
                _planetGenerator.GeneratePlanet();
            }

            /*if (_autoUpdate = GUILayout.Toggle(_autoUpdate, "Auto Update"))
            {
                _planet.GeneratePlanet();
            }*/
        }

        private void DrawSettingsEditor(Object settings, Action onSettingUpdated, ref bool foldout,
            ref UnityEditor.Editor editor)
        {
            if (settings is null)
            {
                return;
            }

            using var check = new EditorGUI.ChangeCheckScope();
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            if (foldout)
            {
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();
            }

            if (check.changed && autoUpdate)
            {
                onSettingUpdated?.Invoke();
            }
        }

        private void OnEnable()
        {
            _planetGenerator = target as PlanetAutoGenerator;
        }
    }
}
