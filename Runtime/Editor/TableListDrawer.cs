using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

[CustomPropertyDrawer(typeof(TableListAttribute))]
public class TableListDrawer : PropertyDrawer
{
    private const float HeaderHeight = 25f;
    private const float RowHeight = 20f;
    private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();
    private Dictionary<string, FieldInfo[]> cachedFields = new Dictionary<string, FieldInfo[]>();
    private Dictionary<string, string[]> cachedHeaders = new Dictionary<string, string[]>();

    private string GetPropertyKey(SerializedProperty property)
    {
        return property.serializedObject.targetObject.GetInstanceID() + "_" + property.propertyPath;
    }

    private bool GetFoldout(SerializedProperty property)
    {
        string key = GetPropertyKey(property);
        if (!foldoutStates.ContainsKey(key))
        {
            foldoutStates[key] = true;
        }
        return foldoutStates[key];
    }

    private void SetFoldout(SerializedProperty property, bool value)
    {
        string key = GetPropertyKey(property);
        foldoutStates[key] = value;
    }

    private SerializedProperty GetActualArrayProperty(SerializedProperty property)
    {
        // If the property is directly an array
        if (property.isArray)
            return property;

        // If it's a List<T>, we need to find the 'Array' property inside it
        var arrayProp = property.FindPropertyRelative("Array");
        if (arrayProp != null)
            return arrayProp;

        // Method 3: Try navigating to the array
        var iterator = property.Copy();
        var end = property.GetEndProperty();
        while (iterator.Next(true) && !SerializedProperty.EqualContents(iterator, end))
        {
            if (iterator.name == "Array" && iterator.isArray)
                return iterator;
        }

        return null;

    }

    private System.Type GetElementType(SerializedProperty property)
    {
        // Get the actual type of the property
        System.Type parentType = property.serializedObject.targetObject.GetType();
        FieldInfo fieldInfo = null;

        // Handle nested properties
        string propertyPath = property.propertyPath;
        string[] pathParts = propertyPath.Split('.');
        string propertyName = pathParts[0];

        fieldInfo = parentType.GetField(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo == null)
            return null;

        var fieldType = fieldInfo.FieldType;

        // Handle array type
        if (fieldType.IsArray)
        {
            return fieldType.GetElementType();
        }
        // Handle List<T> type
        else if (fieldType.IsGenericType &&
                (fieldType.GetGenericTypeDefinition() == typeof(List<>)))
        {
            return fieldType.GetGenericArguments()[0];
        }

        return null;
    }

    private void InitializeIfNeeded(SerializedProperty property)
    {
        string key = GetPropertyKey(property);
        if (cachedFields.ContainsKey(key)) return;

        System.Type elementType = GetElementType(property);

        if (elementType == null)
        {
            Debug.LogError($"TableList attribute on {property.propertyPath} is not attached to an array or list!");
            return;
        }

        // Cache the fields
        var fields = elementType.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => !f.IsLiteral && !f.IsStatic).ToArray();
        cachedFields[key] = fields;

        // Cache the headers
        var tableAttr = attribute as TableListAttribute;
        string[] headers;
        if (tableAttr.UseFieldNamesAsHeaders || tableAttr.Headers == null || tableAttr.Headers.Length == 0)
        {
            headers = fields.Select(f => ObjectNames.NicifyVariableName(f.Name)).ToArray();
        }
        else
        {
            if (tableAttr.Headers.Length != fields.Length)
            {
                Debug.LogWarning($"TableList attribute header count ({tableAttr.Headers.Length}) doesn't match field count ({fields.Length}) for {property.propertyPath}. Using field names instead.");
                headers = fields.Select(f => ObjectNames.NicifyVariableName(f.Name)).ToArray();
            }
            else
            {
                headers = tableAttr.Headers;
            }
        }
        cachedHeaders[key] = headers;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        InitializeIfNeeded(property);

        if (!GetFoldout(property)) return EditorGUIUtility.singleLineHeight;

        string key = GetPropertyKey(property);
        if (!cachedHeaders.ContainsKey(key)) return EditorGUIUtility.singleLineHeight;

        var arrayProp = GetActualArrayProperty(property);
        if (arrayProp == null) return EditorGUIUtility.singleLineHeight;

        return HeaderHeight + (arrayProp.arraySize + 1) * RowHeight + RowHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        InitializeIfNeeded(property);

        string key = GetPropertyKey(property);
        if (!cachedFields.ContainsKey(key) || !cachedHeaders.ContainsKey(key))
        {
            EditorGUI.HelpBox(position, "TableList configuration error. Check console for details.", MessageType.Error);
            return;
        }

        var arrayProp = GetActualArrayProperty(property);
        if (arrayProp == null)
        {
            EditorGUI.HelpBox(position, "Property must be an array or List<T>", MessageType.Error);
            return;
        }

        var fields = cachedFields[key];
        var headers = cachedHeaders[key];

        // Draw foldout and label
        Rect headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        bool foldout = GetFoldout(property);
        foldout = EditorGUI.Foldout(headerRect, foldout, label, true);
        SetFoldout(property, foldout);

        if (!foldout) return;

        // Draw headers
        EditorGUI.indentLevel++;
        float indentWidth = EditorGUI.indentLevel * 15f;
        float columnWidth = (position.width - indentWidth) / headers.Length;

        for (int i = 0; i < headers.Length; i++)
        {
            Rect headerCellRect = new Rect(
                position.x + indentWidth + i * columnWidth,
                position.y + HeaderHeight,
                columnWidth,
                RowHeight
            );
            EditorGUI.LabelField(headerCellRect, headers[i], EditorStyles.boldLabel);
        }

        // Draw elements
        for (int row = 0; row < arrayProp.arraySize; row++)
        {
            var element = arrayProp.GetArrayElementAtIndex(row);

            for (int col = 0; col < fields.Length; col++)
            {
                Rect cellRect = new Rect(
                    position.x + indentWidth + col * columnWidth,
                    position.y + HeaderHeight + (row + 1) * RowHeight,
                    columnWidth,
                    RowHeight
                );

                var fieldProperty = element.FindPropertyRelative(fields[col].Name);
                if (fieldProperty != null)
                {
                    EditorGUI.PropertyField(cellRect, fieldProperty, GUIContent.none);
                }
            }
        }

        // Add/Remove buttons
        //Rect buttonRect = new Rect(
        //    position.x + indentWidth,
        //    position.y + HeaderHeight + (arrayProp.arraySize + 1) * RowHeight,
        //    position.width - indentWidth,
        //    RowHeight
        //);

        //Rect addButtonRect = new Rect(buttonRect.x, buttonRect.y, buttonRect.width / 2 - 2, buttonRect.height);
        //Rect removeButtonRect = new Rect(buttonRect.x + buttonRect.width / 2 + 2, buttonRect.y, buttonRect.width / 2 - 2, buttonRect.height);

        //if (GUI.Button(addButtonRect, "Add Row"))
        //{
        //    arrayProp.arraySize++;
        //    property.serializedObject.ApplyModifiedProperties();
        //}

        //using (new EditorGUI.DisabledGroupScope(arrayProp.arraySize == 0))
        //{
        //    if (GUI.Button(removeButtonRect, "Remove Row"))
        //    {
        //        arrayProp.arraySize--;
        //        property.serializedObject.ApplyModifiedProperties();
        //    }
        //}

        EditorGUI.indentLevel--;
    }
}