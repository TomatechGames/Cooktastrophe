using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SetTextureOnMaterial : MonoBehaviour
{
    [SerializeField]
    Texture2D newTex;
    void Start()
    {
        GetComponent<MeshRenderer>().material.mainTexture = newTex;
    }
}
