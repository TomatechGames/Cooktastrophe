using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabItemComponent : MonoBehaviour
{
    [SerializeField]
    GrabItemReference grabItem;
    [SerializeField]
    MeshFilter meshFilter;
    [SerializeField]
    MeshRenderer meshRenderer;
    [SerializeField]
    BoxCollider boxCollider;

    private void Start()
    {
        if (grabItem.Entry == null)
            return;
        meshFilter.mesh = grabItem.Entry.Mesh;
        if (grabItem.Entry.Materials.Count>0)
        {
            meshRenderer.SetMaterials(grabItem.Entry.Materials);
        }
        meshRenderer.material.mainTexture = grabItem.Entry.DefaultTexture;
        boxCollider.center = grabItem.Entry.Hitbox.center;
        boxCollider.size = grabItem.Entry.Hitbox.size;
    }
}
