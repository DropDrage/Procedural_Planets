using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    [CustomEditor(typeof(Planet), true)]
    public class PlanetEditor : UnityEditor.Editor
    {
        private Planet _planet;

        private UnityEditor.Editor _shapeEditor;
        private UnityEditor.Editor _colourEditor;

        [SerializeField]
        private bool autoUpdate;

        private static bool _shapeFoldout;
        private static bool _colorFoldout;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            autoUpdate = false;

            GUILayout.Space(20);

            DrawSettingsEditor(_planet.shapeSettings, _planet.OnShapeSettingsUpdated, ref _shapeFoldout,
                ref _shapeEditor);
            DrawSettingsEditor(_planet.colorSettings, _planet.OnColorSettingsUpdated, ref _colorFoldout,
                ref _colourEditor);

            GUILayout.Space(20);

            if (GUILayout.Button("Generate Planet"))
            {
                _planet.GeneratePlanet();
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
            _planet = target as Planet;
        }
    }
}