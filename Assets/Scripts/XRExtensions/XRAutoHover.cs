using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRAutoHover : MaterialPropertySetterFloat
{
    void Start()
    {
        PropertyName = "_HoverScale";
        var parentInteractable = GetComponentInParent<XRBaseInteractable>();
        if (parentInteractable)
        {
            parentInteractable.hoverEntered.AddListener((enter) => UpdateHover(enter.interactableObject));
            parentInteractable.hoverExited.AddListener((exit) => UpdateHover(exit.interactableObject));
        }
    }

    public void UpdateHover(IXRHoverInteractable interactable)
    {
        if (interactable.interactorsHovering.Exists(i => i is IPlayerInteractor && !(i is IXRSelectInteractor selector && interactable is IXRSelectInteractable selected && selector.interactablesSelected.Contains(selected))))
            Value = 1;
        else
            Value = 0;
    }
}
