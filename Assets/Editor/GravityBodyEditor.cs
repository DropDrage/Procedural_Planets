using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GravityBody), true)]
    [CanEditMultipleObjects]
    public class GravityBodyEditor : UnityEditor.Editor
    {
        GravityBody gravityObject;
        bool showDebugInfo;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            showDebugInfo = EditorGUILayout.Foldout(showDebugInfo, "Debug info");

            if (showDebugInfo)
            {
                var gravityInfo = GetGravityInfo(gravityObject.transform.position, gravityObject);
                foreach (var t in gravityInfo)
                {
                    EditorGUILayout.LabelField(t);
                }
            }

            /*if (GUILayout.Button("Set Orbit Velocity"))
            {
                // gravityObject.velocity = Mathf.Sqrt(Universe.GravitationConstant * )
            }*/
        }

        private void OnEnable()
        {
            gravityObject = (GravityBody) target;
            showDebugInfo = EditorPrefs.GetBool(gravityObject.gameObject.name + nameof(showDebugInfo), false);
        }

        private void OnDisable()
        {
            if (gravityObject)
            {
                EditorPrefs.SetBool(gravityObject.gameObject.name + nameof(showDebugInfo), showDebugInfo);
            }
        }

        static string[] GetGravityInfo(Vector3 point, GravityBody ignore = null)
        {
            var bodies = GameObject.FindObjectsOfType<GravityBody>();
            var totalAcc = Vector3.zero;

            // gravity
            var forceAndName = new List<FloatAndString>();
            foreach (var body in bodies.Where(body => body != ignore))
            {
                var offsetToBody = body.Position - point;
                var sqrDst = offsetToBody.sqrMagnitude;
                var distance = Mathf.Sqrt(sqrDst);
                var dirToBody = offsetToBody / Mathf.Sqrt(sqrDst);
                var acceleration = Universe.GravitationConstant * body.Mass / sqrDst;
                totalAcc += dirToBody * acceleration;
                forceAndName.Add(new FloatAndString {floatVal = acceleration, stringVal = body.gameObject.name});
            }

            forceAndName.Sort((a, b) => (b.floatVal.CompareTo(a.floatVal)));
            var info = new string[forceAndName.Count + 1];
            info[0] = $"acc: {totalAcc} (mag = {totalAcc.magnitude})";
            for (var i = 0; i < forceAndName.Count; i++)
            {
                info[i + 1] = $"acceleration due to {forceAndName[i].stringVal}: {forceAndName[i].floatVal}";
            }

            return info;
        }

        struct FloatAndString
        {
            public float floatVal;
            public string stringVal;
        }
    }
}