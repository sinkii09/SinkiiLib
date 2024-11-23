using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CanEditMultipleObjects]
[CustomEditor(typeof(object), true)]
public class CustomInspector : Editor
{
    private List<SerializedProperty> _serializedProperties = new List<SerializedProperty>();
    private Dictionary<string, SavedBool> _foldouts = new Dictionary<string, SavedBool>();
    public override void OnInspectorGUI()
    {
        GetSerializedProperties(ref _serializedProperties);
        serializedObject.Update();
        //DrawProperties(target, "myStruct", 0);
        foreach (SerializedProperty prop in _serializedProperties)
        {
           EditorGUILayout.PropertyField(prop);
        }
        serializedObject.ApplyModifiedProperties();
    }
    protected void GetSerializedProperties(ref List<SerializedProperty> outSerializedProperties)
    {
        outSerializedProperties.Clear();
        using var iterator = serializedObject.GetIterator();
        if (iterator.NextVisible(true))
        {
            do
            {
                if (iterator.name == "m_Script") continue;
                SerializedProperty property = serializedObject.FindProperty(iterator.name);
                outSerializedProperties.Add(property);
                AddChildProperties(property, outSerializedProperties);
            }
            while (iterator.NextVisible(false));
        }
    }
    private void AddChildProperties(SerializedProperty parent, List<SerializedProperty> outSerializedProperties)
    {
        // Move the iterator to the end of the parent property
        parent.NextVisible(false);
        SerializedProperty child = parent.Copy();
        do
        {
            // Add the child property to the list
            outSerializedProperties.Add(child.Copy());

            // Recursively add any child properties of the child
            AddChildProperties(child, outSerializedProperties);
        }
        // Iterate through the child properties
        while (child.NextVisible(false));
    }
    private void DrawProperties(object obj, string propertyPath, int depth)
    {
        // Get the type of the object
        var type = obj.GetType();

        // Check if the type is a struct or class
        if (type.IsValueType || type.IsClass)
        {
            // Get the fields of the type
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // Iterate through the fields
            foreach (var field in fields)
            {
                // Check if the field has the RecursiveInspectorAttribute
                var attribute = field.GetCustomAttribute<CustomToggleGroupAttribute>();
                if (attribute != null)
                {
                    // Get the value of the field
                    var fieldValue = field.GetValue(obj);

                    // Draw the field in the inspector
                    UnityEditor.EditorGUILayout.BeginHorizontal();
                    UnityEditor.EditorGUILayout.LabelField(new string(' ', depth * 2) + ObjectNames.NicifyVariableName(field.Name), UnityEditor.EditorStyles.boldLabel);
                    if (attribute.expandByDefault || UnityEditor.EditorGUILayout.Foldout(attribute.expandByDefault, "", true))
                    {
                        // Recursively draw the properties of the field
                        DrawProperties(fieldValue, $"{propertyPath}.{field.Name}", depth + 1);
                    }
                    UnityEditor.EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
    protected void DrawSerializedProperties()
    {

        foreach (var group in GetFoldoutProperties(_serializedProperties))
        {
            if (!_foldouts.ContainsKey(group.Key))
            {
                _foldouts[group.Key] = new SavedBool($"{target.GetInstanceID()}.{group.Key}", false);
            }

            _foldouts[group.Key].Value = EditorGUILayout.Foldout(_foldouts[group.Key].Value, group.Key, true);
            if (_foldouts[group.Key].Value)
            {
                foreach (var property in group)
                {
                    CustomEditorGUI.PropertyField_Layout(property, true);
                }
            }
        }
    }
    private static IEnumerable<IGrouping<string, SerializedProperty>> GetFoldoutProperties(IEnumerable<SerializedProperty> properties)
    {
        return properties
            .Where(p => PropertyUtility.GetAttribute<CustomToggleGroupAttribute>(p) != null)
            .GroupBy(p => PropertyUtility.GetAttribute<CustomToggleGroupAttribute>(p).Name);
    }
}
