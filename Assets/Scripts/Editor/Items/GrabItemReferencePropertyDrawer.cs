using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(GrabItemReferenceAttribute))]
public class GrabItemReferencePropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        GrabItemReferenceAttribute refAttribute = attribute as GrabItemReferenceAttribute;
        VisualElement rootElement;
        VisualElement contentElement;
        Box inlineInspectorBox = null;
        GrabItemDatabase database = Resources.FindObjectsOfTypeAll<GrabItemDatabase>().FirstOrDefault();

        if (refAttribute.InlineInspector)
        {
            var rootFoldout = new Foldout() { text = "" };
            contentElement = new VisualElement() { style = { marginLeft = -3, flexGrow = 1, flexDirection = FlexDirection.Row } };
            rootFoldout.Q<Toggle>().Children().First().Add(contentElement);

            //add inline inspector
            inlineInspectorBox = new Box();
            rootFoldout.Q("unity-content").Add(inlineInspectorBox);

            if (database)
                CreateInlineInspector(inlineInspectorBox, property.intValue, database);

            rootFoldout.value = false;
            rootElement = rootFoldout;
        }
        else
        {
            rootElement = new VisualElement() { style = { flexGrow = 1, flexDirection = FlexDirection.Row } };
            contentElement = rootElement;
        }
        rootElement.name = "root";

        //add dropdown selector
        var nameField = new TextField {label=property.displayName, style = { paddingLeft = 1, flexGrow = 1 } };

        nameField.AddToClassList(BaseField<int>.alignedFieldUssClassName);
        if (database)
            BindName(database, nameField, property.intValue);
        nameField.Q("unity-text-input").SetEnabled(false);
        contentElement.Add(nameField);

        rootElement.RegisterCallback<AttachToPanelEvent>(e =>
        {
            if (rootElement.parent is PropertyField field)
            {
                if(field.label is not null)
                    nameField.label = field.label;
                field.RegisterCallback<SerializedPropertyChangeEvent>(f =>
                {
                    if (database)
                        BindName(database, nameField, property.intValue);
                });
            }
        });

        var searchButton = new Button() { style = { flexGrow = 0, flexShrink = 0, width = 18, marginLeft = 3 } };
        searchButton.clicked += () =>
        {
            if (database)
            {
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(searchButton.worldBound.center)),

                    ScriptableObject.CreateInstance<GenericSearchProvider>()

                    .SetChoices(database.ItemEntries.Select(e => e as object).ToList(), true)

                    .SetKeyGenerator((e) => (e as GrabItemEntry).Name)

                    .SetActionOnSelect(e =>
                    {
                        var entry = e as GrabItemEntry;
                        property.intValue = entry.Id;
                        if (inlineInspectorBox is not null)
                            CreateInlineInspector(inlineInspectorBox, property.intValue, database);
                        BindName(database, nameField, property.intValue);
                        property.serializedObject.ApplyModifiedProperties();
                    }));
            }
        };
        contentElement.Add(searchButton);

        return rootElement;
    }

    void BindName(GrabItemDatabase database, TextField nameField, int id)
    {
        var entry = GetItemProperty(database, id);
        if (entry != null)
        {
            nameField.BindProperty(entry.FindPropertyRelative("name"));
        }
    }

    SerializedProperty GetItemProperty(GrabItemDatabase database, int id){
        int index = database.EditorIndexOfEntry(id);
        if (index == -1)
            return null;
        return new SerializedObject(database).FindProperty("m_ItemEntries").GetArrayElementAtIndex(index);
    }

    void CreateInlineInspector(VisualElement inlineInspectorParent, int id, GrabItemDatabase database)
    {
        inlineInspectorParent.Clear();

        var entry = GetItemProperty(database, id);
        if (entry == null)
            return;

        var field = new PropertyField();
        field.BindProperty(entry);
        inlineInspectorParent.Add(field);
    }
}
