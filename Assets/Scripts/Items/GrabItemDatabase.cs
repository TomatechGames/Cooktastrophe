using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CreateAssetMenu(menuName = "Tomatech/GrabItem Database")]
public class GrabItemDatabase : ScriptableObject
{
    [SerializeField]
    List<GrabItemEntry> m_ItemEntries = new();
    [SerializeField]
    List<CombinationRecipe> m_CombinationRecipes = new();
    [SerializeField]
    List<ProcessRecipe> m_ProcessRecipes = new();
    Dictionary<int, GrabItemEntry> m_ItemDictionary;

    public List<GrabItemEntry> ItemEntries => m_ItemEntries.ToList();

    private void OnValidate()
    {
        GenerateDictionary();
    }

    public List<CombinationRecipe> GetCombinations(int filter) => m_CombinationRecipes
        .Where(c => c.IngredientA == filter || c.IngredientB == filter || c.Result == filter)
        .ToList();
    public CombinationRecipe GetCombinationEntry(int filterA, int filterB) => m_CombinationRecipes
        .FirstOrDefault(c => (c.IngredientA == filterA && c.IngredientB == filterB));

    public List<ProcessRecipe> GetProcesses(int filter) => m_ProcessRecipes
        .Where(c => c.Ingredient == filter || c.Result == filter)
        .ToList();
    public ProcessRecipe GetProcessEntry(int filter, ProcessType process) => m_ProcessRecipes
        .FirstOrDefault(c => c.Ingredient == filter && c.Process == process);

    bool GenerateDictionary()
    {
        if (m_ItemEntries.Count == 0)
            return false;

        m_ItemEntries
            .Where(i=>i.Id==0 || m_ItemEntries.Where(j=>i.Id==j.Id).Count()>1)
            .ToList()
            .ForEach(i=>i.RegenerateID(m_ItemEntries
                .Where(j=>j.Id!=i.Id)
                .Select(i=>i.Id)
                .ToList()
            ));
        m_ItemDictionary = m_ItemEntries.ToDictionary(e=>e.Id);
        return true;
    }

    public int EditorIndexOfEntry(int i)
    {
        if (!GenerateDictionary() || !m_ItemDictionary.ContainsKey(i))
            return -1;
        return m_ItemEntries.IndexOf(m_ItemDictionary[i]);
    }

    public GrabItemEntry this[int i]
    {
        get
        {
            if ((m_ItemDictionary is null && !GenerateDictionary()) || !m_ItemDictionary.ContainsKey(i))
                return null;
            return m_ItemDictionary[i];
        }
    }

    class CombinationComparer : IEqualityComparer<CombinationRecipe>
    {
        public bool Equals(CombinationRecipe x, CombinationRecipe y)
        {
            int x1 = Mathf.Min(x.IngredientA, x.IngredientB);
            int x2 = Mathf.Max(x.IngredientA, x.IngredientB);
            int y1 = Mathf.Min(y.IngredientA, y.IngredientB);
            int y2 = Mathf.Max(y.IngredientA, y.IngredientB);
            return x1 == y1 && x2 == y2;
        }

        public int GetHashCode(CombinationRecipe obj)
        {
            return obj.IngredientA ^ obj.IngredientB;
        }
    }
}

public class GrabItemIDReferenceAttribute : PropertyAttribute
{
    bool inlineInspector;
    public bool InlineInspector => inlineInspector;
    public GrabItemIDReferenceAttribute(bool inlineInspector = false)
    {
        this.inlineInspector = inlineInspector;
    }
}

[System.Serializable]
public class GrabItemReference
{
    [SerializeField]
    [GrabItemIDReference(true)]
    int id;
    public int Id
    {
        get
        {
            hasEntry = false;
            return id;
        }
        set
        {
            id = value;

        }
    }
    bool hasEntry;
    GrabItemEntry entry;
    public GrabItemEntry Entry
    {
        get
        {
            if (hasEntry)
                return entry;

            if (!GrabItemDatabaseHolder.Database)
                entry = null;
            else
                entry = GrabItemDatabaseHolder.Database[id];

            hasEntry = true;
            return entry;
        }
    }
}

[System.Serializable]
public class GrabItemEntry
{
    [SerializeField]
    string name;
    public string Name => name;

    [SerializeField]
    Mesh mesh;
    public Mesh Mesh => mesh;

    [SerializeField]
    List<Material> materials;
    public List<Material> Materials => materials;

    [SerializeField]
    Texture2D defaultTexture;
    public Texture2D DefaultTexture => defaultTexture;

    [SerializeField]
    Vector3 hitbox = Vector3.one;
    public Vector3 Hitbox => hitbox;

    [SerializeField]
    int id;
    public int Id => id;
    public void RegenerateID(List<int> existingIDs)
    {
        while (existingIDs.Contains(id) || id==0)
        {
            id = Random.Range(short.MinValue, short.MaxValue);
        }
    }
}

[System.Serializable]
public class CombinationRecipe
{
    [GrabItemIDReference, SerializeField]
    int ingredientA;
    public int IngredientA => ingredientA;

    [GrabItemIDReference, SerializeField]
    int ingredientB;
    public int IngredientB => ingredientB;

    [GrabItemIDReference, SerializeField]
    int result;
    public int Result => result;

    public bool InvolvesItem(int itemID)
    {
        return IngredientA == itemID || IngredientB == itemID || Result == itemID;
    }
}

public enum ProcessType
{
    Heat,
    Water,
    Mould,
    Chop
}

[System.Serializable]
public class ProcessRecipe
{
    [GrabItemIDReference, SerializeField]
    int ingredient;
    public int Ingredient => ingredient;

    [SerializeField]
    ProcessType process;
    public ProcessType Process => process;

    [GrabItemIDReference, SerializeField]
    int result;
    public int Result => result;

    [SerializeField]
    bool badRecipe;
    public bool BadRecipe => badRecipe;

    public bool InvolvesItem(int itemID)
    {
        return Ingredient == itemID  || Result == itemID;
    }
}

public static class GrabItemUtils
{
    public static CombinationRecipe TryGetCombination(int itemA, int itemB)
    {
        var recipe = GrabItemDatabaseHolder.Database.GetCombinationEntry(itemA, itemB);
        recipe ??= GrabItemDatabaseHolder.Database.GetCombinationEntry(itemB, itemA);
        return recipe;
    }

    public static bool TryCombine(GrabItemComponent sourceGrabItem, GrabItemComponent destGrabItem)
    {
        var recipe = TryGetCombination(sourceGrabItem.GrabItem.Id, destGrabItem.GrabItem.Id);

        if (recipe == null)
            return false;

        destGrabItem.SetNewItemID(recipe.Result);
        Object.Destroy(sourceGrabItem.gameObject);

        return true;
    }

    public static List<XRSocketInteractor> GetSocketEndpoints(MonoBehaviour root, bool withChild = false)
    {
        List<XRSocketInteractor> result = new();
        GetSocketEndpoints(root, ref result, withChild);
        return result;
    }
    static void GetSocketEndpoints(MonoBehaviour current, ref List<XRSocketInteractor> totalEndpoints, bool withChild)
    {
        var currentSockets = current.GetComponentsInChildren<XRSocketInteractor>().ToList();
        foreach (var item in currentSockets)
        {
            if (withChild == (item.interactablesSelected.Count != 0))
                totalEndpoints.Add(item);
            if (item.interactablesSelected.Count != 0)
                GetSocketEndpoints(item.firstInteractableSelected as MonoBehaviour, ref totalEndpoints, withChild);
        }
    }

    public static List<IXRSelectInteractable> GetSelectableEndpoints(MonoBehaviour root)
    {
        List<IXRSelectInteractable> result = new();
        GetSelectableEndpoints(root, ref result);
        result.Remove(root as IXRSelectInteractable);
        return result;
    }
    static void GetSelectableEndpoints(MonoBehaviour current, ref List<IXRSelectInteractable> totalEndpoints)
    {
        var currentSockets = current.GetComponentsInChildren<XRSocketInteractor>();
        bool isEndpoint = true;
        foreach (var item in currentSockets)
        {
            if (item.interactablesSelected.Count != 0)
            {
                GetSelectableEndpoints(item.firstInteractableSelected as MonoBehaviour, ref totalEndpoints);
                isEndpoint = false;
            }
        }
        if (isEndpoint)
            totalEndpoints.Add(current as IXRSelectInteractable);
    }
}