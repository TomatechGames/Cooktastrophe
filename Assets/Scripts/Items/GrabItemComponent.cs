using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabItemComponent : MonoBehaviour
{
    static Dictionary<IXRSelectInteractable, GrabItemComponent> selectableMap = new();

    public static GrabItemComponent Remap(IXRSelectInteractable selectable) =>
        selectableMap.ContainsKey(selectable) ? selectableMap[selectable] : null;
    public static bool TryRemap(IXRSelectInteractable selectable, out GrabItemComponent grabItem) =>
        grabItem = Remap(selectable);

    IXRSelectInteractable linkedSelectable;
    IXRSelectInteractable LinkedSelectable => 
        (linkedSelectable as MonoBehaviour) ? linkedSelectable : (linkedSelectable = GetComponent<IXRSelectInteractable>());
    [SerializeField]
    GrabItemReference grabItem;
    public GrabItemReference GrabItem => grabItem;
    [SerializeField]
    MeshFilter meshFilter;
    [SerializeField]
    MeshRenderer meshRenderer;
    [SerializeField]
    BoxCollider boxCollider;


    private void Start()
    {
        linkedSelectable = LinkedSelectable;
        ApplyItem();
    }

    private void OnEnable()
    {
        if(LinkedSelectable as MonoBehaviour && !selectableMap.ContainsKey(LinkedSelectable))
            selectableMap.Add(LinkedSelectable, this);
        //Debug.Log(string.Join(", ", selectableMap.Select(m => $"[{m.Key}:{m.Value}]")));
    }

    private void OnDisable()
    {
        if(LinkedSelectable as MonoBehaviour && selectableMap.ContainsKey(LinkedSelectable))
            selectableMap.Remove(LinkedSelectable);
    }

    public void SetNewItemID(int newID)
    {
        grabItem.Id = newID;
        ApplyItem();
    }

    void ApplyItem()
    {
        if (grabItem.Entry == null)
        {
            meshFilter.mesh = null;
            return;
        }
        meshFilter.mesh = grabItem.Entry.Mesh;
        if (grabItem.Entry.Materials.Count > 0)
        {
            meshRenderer.SetMaterials(grabItem.Entry.Materials);
        }
        meshRenderer.material.mainTexture = grabItem.Entry.DefaultTexture;
        boxCollider.center = 0.5f * grabItem.Entry.Hitbox.y * Vector3.up;
        boxCollider.size = grabItem.Entry.Hitbox;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + (Vector3.up * 0.02f), 0.02f);
    }


}
