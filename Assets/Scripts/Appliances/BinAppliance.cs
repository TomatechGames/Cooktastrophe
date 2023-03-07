using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BinAppliance : MonoBehaviour
{
    XRSocketInteractor binSocket;

    // Start is called before the first frame update
    void Start()
    {
        binSocket = GetComponentInChildren<XRSocketInteractor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (binSocket.hasSelection)
        {
            Destroy((binSocket.firstInteractableSelected as MonoBehaviour).gameObject, 0.1f);
        }
    }
}
