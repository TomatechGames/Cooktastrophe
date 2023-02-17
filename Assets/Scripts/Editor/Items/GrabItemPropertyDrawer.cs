using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(GrabItemEntry))]
public class GrabItemPropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new();
        GrabItemDatabase database = Resources.FindObjectsOfTypeAll<GrabItemDatabase>().FirstOrDefault();
        int id = property.FindPropertyRelative("id").intValue;

        VisualElement propertyPanel= new();
        propertyPanel.Add(new PropertyField(property.FindPropertyRelative("name")));
        propertyPanel.Add(new PropertyField(property.FindPropertyRelative("mesh")));
        propertyPanel.Add(new PropertyField(property.FindPropertyRelative("materials")));


        VisualElement recipePanel = new() { style = { display = DisplayStyle.None } };
        ListView combinationList = new();
        combinationList.makeItem = () =>
        {
            var field = new PropertyField();
            field.SetEnabled(false);
            return field;
        };
        combinationList.bindItem = (e, i) =>
        {
            var propAtIndex = (combinationList.itemsSource as IList<SerializedProperty>)[i];
            (e as PropertyField).BindProperty(propAtIndex);
        };
        ListView processList = new();
        processList.makeItem = () =>
        {
            var field = new PropertyField();
            field.SetEnabled(false);
            return field;
        };
        processList.bindItem = (e, i) =>
        {
            var propAtIndex = (processList.itemsSource as IList<SerializedProperty>)[i];
            (e as PropertyField).BindProperty(propAtIndex);
        };

        if (database)
        {
            List<SerializedProperty> filteredProps= new();
            SerializedProperty combinationArray = new SerializedObject(database).FindProperty("m_CombinationRecipes");
            for (int i = 0; i < combinationArray.arraySize; i++)
            {
                SerializedProperty potentialProp = combinationArray.GetArrayElementAtIndex(i);
                if(potentialProp.boxedValue is CombinationRecipe recipe && recipe.InvolvesItem(id))
                    filteredProps.Add(potentialProp);
            }
            combinationList.itemsSource = filteredProps.ToList();

            filteredProps.Clear();
            SerializedProperty processArray = new SerializedObject(database).FindProperty("m_ProcessRecipes");
            for (int i = 0; i < processArray.arraySize; i++)
            {
                SerializedProperty potentialProp = processArray.GetArrayElementAtIndex(i);
                if (potentialProp.boxedValue is ProcessRecipe recipe && recipe.InvolvesItem(id))
                    filteredProps.Add(potentialProp);
            }
            processList.itemsSource = filteredProps.ToList();
        }

        Toolbar tabParent = new() { style = { borderLeftWidth = 1, borderTopWidth = 1 } };

        ToolbarToggle propertyTab = new() {name="properties", style = { left = new StyleLength(StyleKeyword.Auto), flexShrink = 1 , flexGrow=1}, text="Properties", userData = propertyPanel};
        ToolbarToggle recipeTab = new() {name="recipes", style = { left = new StyleLength(StyleKeyword.Auto), flexShrink = 1, flexGrow=1 }, text="Recipes", userData = recipePanel};

        propertyTab.value = true;

        propertyTab.RegisterCallback<ClickEvent>(EnforceCurrentTab);
        propertyTab.RegisterValueChangedCallback(SetLinkedPanel);

        recipeTab.RegisterCallback<ClickEvent>(EnforceCurrentTab);
        recipeTab.RegisterValueChangedCallback(SetLinkedPanel);

        tabParent.Add(propertyTab);
        tabParent.Add(recipeTab);
        recipePanel.Add(combinationList);
        recipePanel.Add(processList);
        root.Add(tabParent);
        root.Add(propertyPanel);
        root.Add(recipePanel);
        return root;
    }

    void SetLinkedPanel(ChangeEvent<bool> e)
    {
        //Debug.Log("Apply " + e.target);
        if ((e.target as VisualElement).userData is VisualElement panel)
            panel.style.display = e.newValue ? DisplayStyle.Flex : DisplayStyle.None;
    }

    void EnforceCurrentTab(ClickEvent e)
    {
        //Debug.Log("Ensure "+ (e.target as ToolbarToggle));
        var siblings = (e.target as VisualElement).parent
            .Query<ToolbarToggle>()
            .Where(t=>t!=e.target as ToolbarToggle)
            .ToList();
        //Debug.Log(string.Join(",",siblings));
        siblings.ForEach(t=>t.value=false);
    }
}
