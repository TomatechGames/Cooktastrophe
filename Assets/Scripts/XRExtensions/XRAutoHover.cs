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
            parentInteractable.hoverEntered.AddListener((HoverEnterEventArgs) => Value = 0.25f);
            parentInteractable.hoverExited.AddListener((HoverExitEventArgs) => Value = 0f);
        }
    }
}
