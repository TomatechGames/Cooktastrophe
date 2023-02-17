using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public abstract class MaterialPropertySetterBase : MonoBehaviour
{
    MeshRenderer m_Renderer;
    protected MeshRenderer Renderer { 
        get
        {
            if(!m_Renderer)
                m_Renderer = GetComponent<MeshRenderer>();
            return m_Renderer;
        } 
    }

    [field: SerializeField]
    public string PropertyName { get; protected set; }
}

public class MaterialPropertySetterFloat : MaterialPropertySetterBase
{
    public float Value
    {
        get
        {
            return Renderer.material.GetFloat(PropertyName);
        }
        set
        {
            Renderer.material.SetFloat(PropertyName, value);
        }
    }
}



