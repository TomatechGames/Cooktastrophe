using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ProcessRecipe))]
public class ProcessRecipePropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new() { style = { flexDirection = FlexDirection.Row, flexGrow = 0 } };
        root.Add(CreateLayoutProp(property, "ingredient"));
        root.Add(new Label("+"));
        root.Add(CreateLayoutProp(property, "process"));
        root.Add(new Label("  ="));
        root.Add(CreateLayoutProp(property, "result"));
        root.Add(new Label("("));
        root.Add(new PropertyField(property.FindPropertyRelative("badRecipe"), "") { style = {left=-3, right=-3}});
        root.Add(new Label(")"));
        return root;
    }

    PropertyField CreateLayoutProp(SerializedProperty rootProp, string propertyName)
    {
        var prop = rootProp.FindPropertyRelative(propertyName);
        var propField = new PropertyField(prop, "") { style = { flexGrow = 1 } };
        propField.BindProperty(prop);
        propField.generateVisualContent += m =>
        {
            var rootElement = propField.Q("root");
            if (rootElement == null)
                return;
            //Debug.Log(rootElement);
            rootElement.style.position = Position.Absolute;
            rootElement.style.left = 0;
            rootElement.style.right = 0;
            //rootElement.style.width = new Length(0, LengthUnit.Percent);
            //rootElement.style.height = new Length(100, LengthUnit.Percent);
        };
        return propField;
    }
}
