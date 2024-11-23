using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CustomEditorGUI
{
    private delegate void PropertyFieldFunction(Rect rect, SerializedProperty property, GUIContent label, bool includeChildren);
    public static void PropertyField_Layout(SerializedProperty property, bool includeChildren)
    {
        Rect dummyRect = new Rect();
        PropertyField_Implementation(dummyRect, property, includeChildren, DrawPropertyField_Layout);
    }
    private static void DrawPropertyField_Layout(Rect rect, SerializedProperty property, GUIContent label, bool includeChildren)
    {
        EditorGUILayout.PropertyField(property, label, includeChildren);
    }
    private static void PropertyField_Implementation(Rect rect, SerializedProperty property, bool includeChildren, PropertyFieldFunction propertyFieldFunction)
    {
        Rect headerRect = new Rect(rect.x, rect.y, rect.width - 30, 24f);
        Rect toggleRect = new Rect(rect.x + 5, rect.y + 2, 20, 20);
        Event evt = Event.current;

        // Check if enabled and draw
        EditorGUI.BeginChangeCheck();
        //if (evt.type == EventType.Repaint)
        //{
        //    EditorGUI.DrawRect(headerRect, new Color(0.2f, 0.2f, 0.2f));
        //    bool isToggled = GUI.Toggle(toggleRect, true, GUIContent.none, EditorStyles.miniButton);
        //    //if (isToggled != savedToggle.Value)
        //    //{
        //    //    EditorUtility.SetDirty(property.serializedObject.targetObject);
        //    //}

        //    //// Draw the header label
        //    //GUI.Label(headerRect, group.Key, headerStyle);
        //}
        bool enabled = true;

        using (new EditorGUI.DisabledScope(disabled: !enabled))
        {
            propertyFieldFunction.Invoke(rect, property, new GUIContent(property.displayName), includeChildren);
        }

        EditorGUI.EndChangeCheck();
    }
}
internal class SavedBool
{
    private bool _value;
    private string _name;

    public bool Value
    {
        get
        {
            return _value;
        }
        set
        {
            if (_value == value)
            {
                return;
            }

            _value = value;
            EditorPrefs.SetBool(_name, value);
        }
    }

    public SavedBool(string name, bool value)
    {
        _name = name;
        _value = EditorPrefs.GetBool(name, value);
    }
}
