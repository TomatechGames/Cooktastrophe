using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTextureOnMaterial : MonoBehaviour
{
    [SerializeField]
    Texture2D newTex;
    void Start()
    {
        GetComponentInChildren<MeshRenderer>().material.mainTexture = newTex;
    }
}
