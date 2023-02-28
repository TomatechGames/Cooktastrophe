using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ApplianceSocketBlocker : MonoBehaviour
{
    XRSocketInteractor socketInteractor;
    List<Collider> currentCollisions=new();
    [SerializeField]
    LayerMask playerLayer;
    private void Start()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        currentCollisions.Add(other);
        UpdateTriggers();
    }

    private void OnTriggerExit(Collider other)
    {
        currentCollisions.Remove(other);
        UpdateTriggers();
    }

    private void UpdateTriggers()
    {
        if (socketInteractor && !socketInteractor.hasSelection)
            socketInteractor.socketActive =!currentCollisions.Exists(x => playerLayer.Contains(x.gameObject.layer));
    }
}
