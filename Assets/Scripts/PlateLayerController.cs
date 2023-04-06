using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlateLayerController : MonoBehaviour
{

    private void Start()
    {
        var socket = GetComponentInChildren<XRSocketInteractor>();
        var grabable = GetComponent<XRGrabInteractable>();
        socket.selectEntered.AddListener(e =>
        {
            grabable.interactionLayers = InteractionLayerMask.GetMask("Item", "OccupiedPlate");
        });
        socket.selectExited.AddListener(e =>
        {
            grabable.interactionLayers = InteractionLayerMask.GetMask("Item", "Plate");
        });
    }
}
