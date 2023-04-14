using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ApplianceHoverObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var socket = GetComponentInParent<XRSocketInteractor>();
        socket.hoverEntered.AddListener(e =>
        {
            if (!socket.interactablesSelected.Contains(e.interactableObject as IXRSelectInteractable))
                transform.localScale = Vector3.one;
        });
        socket.selectEntered.AddListener(e =>
        {
            transform.localScale = Vector3.zero;
        });
        socket.selectExited.AddListener(e =>
        {
            transform.localScale = Vector3.one;
        });
        socket.hoverExited.AddListener(e =>
        {
            transform.localScale = Vector3.zero;
        });
    }
}
