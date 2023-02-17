using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabItemComponent : MonoBehaviour
{
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
        ApplyItem();
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
