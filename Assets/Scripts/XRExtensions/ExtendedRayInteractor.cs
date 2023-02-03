using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ExtendedRayInteractor : XRRayInteractor, IPlayerInteractor
{
    public override bool CanHover(IXRHoverInteractable interactable)
    {
        if (interactable is IXRSelectInteractable selectable && !ValidateSelectable(selectable))
        {
            return false;
        }
        return base.CanHover(interactable);
    }

    public override bool CanSelect(IXRSelectInteractable selectable)
    {
        if (!ValidateSelectable(selectable))
        {
            return false;
        }
        return base.CanSelect(selectable);
    }
    //protected override void OnSelectEntering(SelectEnterEventArgs args)
    //{
    //    bool prevUseForceGrab = useForceGrab;
    //    base.OnSelectEntering(args);
    //    useForceGrab = prevUseForceGrab;
    //}

    bool ValidateSelectable(IXRSelectInteractable selectable) => ExtendedDirectInteractor.ValidateSelectable(selectable, this);
}
