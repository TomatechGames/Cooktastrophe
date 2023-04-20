using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(CombinationRecipe))]
public class CombinationRecipePropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new() { style = { flexDirection = FlexDirection.Row, flexGrow = 0}};
        root.Add(CreateIngredientProp(property, "ingredientA"));
        root.Add(new Label("+"));
        root.Add(CreateIngredientProp(property, "ingredientB"));
        root.Add(new Label("="));
        root.Add(CreateIngredientProp(property, "result"));
        return root;
    }

    PropertyField CreateIngredientProp(SerializedProperty rootProp, string propertyName)
    {
        var prop = rootProp.FindPropertyRelative(propertyName);
        var propField = new PropertyField(prop, "") { style = {flexGrow=1}};
        propField.BindProperty(prop);
        propField.generateVisualContent += m =>
        {
            var rootElement = propField.Q("root");
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