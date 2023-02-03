using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

public class DestroyOnPlatformMismatch : MonoBehaviour
{
    [SerializeField]
    bool isPancake;

    void Start()
    {
        if(XRGeneralSettings.Instance.Manager.activeLoader == isPancake)
            Destroy(gameObject);
    }
}
