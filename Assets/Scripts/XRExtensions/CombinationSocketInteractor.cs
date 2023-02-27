using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class CombinationSocketInteractor : XRSocketInteractor
{
    [SerializeField]
    XRBaseInteractor connectedInteractor;

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        if(interactable is IXRSelectInteractable selectable && ValidateSelectable(selectable) && !selectable.interactorsSelecting.Exists(i=>i is XRSocketInteractor))
            return base.CanHover(interactable);
        return false;
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        if(!interactable.isSelected && ValidateSelectable(interactable))
            return base.CanSelect(interactable);
        return false;
    }

    // only allow items when the linked interactor has an item and the item can be combined with hovered items
    bool ValidateSelectable(IXRSelectInteractable interactable)
    {
        if(!connectedInteractor || !connectedInteractor.hasSelection)
            return false;

        bool hasCombiners = GrabItemComponent.TryRemap(connectedInteractor.firstInteractableSelected, out var destGrabComponent);
        hasCombiners &= GrabItemComponent.TryRemap(interactable, out var sourceGrabComponent);

        if (hasCombiners)
        {
            return GrabItemUtils.TryGetCombination(sourceGrabComponent.GrabItem.Id, destGrabComponent.GrabItem.Id) != null;
        }
        return false;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // combine current item into interactor item after a delay (0.1s)
        if (connectedInteractor.firstInteractableSelected == null || args.interactableObject == null)
            return;
        bool hasCombiners = GrabItemComponent.TryRemap(connectedInteractor.firstInteractableSelected, out var destGrabComponent);
        hasCombiners &= GrabItemComponent.TryRemap(args.interactableObject, out var sourceGrabComponent);
        if (hasCombiners)
            StartCoroutine(DelayedInvoke(0.1f, () => {
                if(sourceGrabComponent && destGrabComponent)
                GrabItemUtils.TryCombine(sourceGrabComponent, destGrabComponent);
                }));
    }

    IEnumerator DelayedInvoke(float waitTime, System.Action run)
    {
        yield return new WaitForSeconds(waitTime);
        run?.Invoke();
    }
}
