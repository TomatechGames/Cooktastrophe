using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionSearchProvider : ScriptableObject, ISearchWindowProvider
{
    Action<InputActionReference> onSelectEntry;
    public InputActionSearchProvider SetActionOnSelect(Action<InputActionReference> onSelectEntry)
    {
        this.onSelectEntry = onSelectEntry;
        return this;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchList = new()
        {
            new SearchTreeGroupEntry(new GUIContent("List"), 0)
        };

        Dictionary<string, InputActionReference> keyValuePairs = new();

        Resources.FindObjectsOfTypeAll<InputActionAsset>()
        .SelectMany(a => AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(a))
        .Where(a => a is not InputActionAsset)
        .Select(a => a as InputActionReference))
        .ToList()
        .ForEach(a =>
        {
            string key = a.name;
            if(!keyValuePairs.ContainsKey(key))
                keyValuePairs.Add(key, a);
        });
        List<string> entryList = keyValuePairs.Keys.ToList();

        entryList.Sort((a, b) =>
        {
            var splitA = a.Split("/");
            var splitB = b.Split("/");
            for (int i = 0; i < splitA.Length; i++)
            {
                if (i >= splitB.Length)
                {
                    return i;
                }
                int value = splitA[i].CompareTo(splitB[i]);
                if (value != 0)
                {
                    if (splitA.Length != splitB.Length && (i == splitA.Length - 1 || i == splitB.Length - 1))
                        return splitA.Length < splitB.Length ? 1 : -1;
                    return value;
                }
            }
            return 0;
        });

        List<string> groupList = new();
        foreach (string key in entryList)
        {
            var entryTitle = key.Split("/");
            string groupName = "";
            for (int i = 0; i < entryTitle.Length - 1; i++)
            {
                groupName += entryTitle[i];
                if (!groupList.Contains(groupName))
                {
                    searchList.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                    groupList.Add(groupName);
                }
                groupName += "/";
            }
            SearchTreeEntry entry = new(new GUIContent(entryTitle.Last()))
            {
                level = entryTitle.Length,
                userData = keyValuePairs[key]
            };
            searchList.Add(entry);
        }

        return searchList;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        onSelectEntry?.Invoke(SearchTreeEntry.userData as InputActionReference);
        return true;
    }
}
