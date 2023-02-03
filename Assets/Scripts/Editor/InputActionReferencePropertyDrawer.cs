using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(InputActionReference))]
public class InputActionReferencePropertyDrawer : PropertyDrawer
{
    //public override VisualElement CreatePropertyGUI(SerializedProperty property)
    //{
    //    var root = new VisualElement() { style = { flexDirection = FlexDirection.Row } };

    //    var objectField = new ObjectField();
    //    objectField.objectType = typeof(InputActionReference);
    //    objectField.AddToClassList(BaseField<InputActionReference>.alignedFieldUssClassName);
    //    root.Add(objectField);

    //    var searchButton = new Button() { style = { width = 22 } };
    //    searchButton.clicked += () =>
    //    {

    //    };
    //    root.Add(searchButton);

    //    return root;
    //}

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect baseRect = position;
        baseRect.width -= baseRect.height;
        Rect newRect = position;
        newRect.width = newRect.height;
        newRect.x += baseRect.width;

        EditorGUI.ObjectField(baseRect, property, label);

        if (GUI.Button(newRect, EditorGUIUtility.IconContent("SearchDatabase Icon")))
        {
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(newRect.center)),
                    ScriptableObject.CreateInstance<InputActionSearchProvider>()
                    .SetActionOnSelect(e =>
                    {
                        property.objectReferenceValue = e;
                        property.serializedObject.ApplyModifiedProperties();
                    }));
        }
    }
}
