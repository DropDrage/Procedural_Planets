using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor
{
    //Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
    //Modified by: Sebastian Lague

    [CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
    public class ConditionalHidePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var condHAtt = (ConditionalHideAttribute) attribute;
            var enabled = GetConditionalHideAttributeResult(condHAtt, property);

            if (enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var condHAtt = (ConditionalHideAttribute) attribute;
            var enabled = GetConditionalHideAttributeResult(condHAtt, property);

            if (enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }

            //We want to undo the spacing added before and after the property
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        private static bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt,
            SerializedProperty property)
        {
            SerializedProperty sourcePropertyValue;

            //Get the full relative property path of the sourcefield so we can have nested hiding.Use old method when dealing with arrays
            if (!property.isArray)
            {
                //returns the property path of the property we want to apply the attribute to
                var propertyPath = property.propertyPath;
                //changes the path to the conditionalsource property path
                var conditionPath = propertyPath.Replace(property.name, condHAtt.conditionalSourceField);

                sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

                //if the find failed->fall back to the old system
                if (sourcePropertyValue is null)
                {
                    //original implementation (doens't work with nested serializedObjects)
                    sourcePropertyValue = property.serializedObject.FindProperty(condHAtt.conditionalSourceField);
                }
            }
            else
            {
                //original implementation (doens't work with nested serializedObjects)
                sourcePropertyValue = property.serializedObject.FindProperty(condHAtt.conditionalSourceField);
            }


            return sourcePropertyValue is null || CheckPropertyType(condHAtt, sourcePropertyValue);
        }

        private static bool CheckPropertyType(ConditionalHideAttribute condHAtt, SerializedProperty sourcePropertyValue)
        {
            //Note: add others for custom handling if desired
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (sourcePropertyValue.propertyType)
            {
                case SerializedPropertyType.Enum:
                    return sourcePropertyValue.enumValueIndex == condHAtt.enumIndex;
                case SerializedPropertyType.Boolean:
                    return sourcePropertyValue.boolValue;
                default:
                    Debug.LogError("Data type of the property used for conditional hiding ["
                                   + sourcePropertyValue.propertyType
                                   + "] is currently not supported");
                    return true;
            }
        }
    }
}
