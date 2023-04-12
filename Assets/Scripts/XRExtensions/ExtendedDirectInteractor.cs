using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ExtendedDirectInteractor : XRDirectInteractor, IPlayerInteractor, ICombinableInteractor
{
    public override bool CanHover(IXRHoverInteractable interactable)
    {
        if (interactable is IXRSelectInteractable selectable && !ValidateSelectable(selectable, this))
        {
            return false;
        }
        return base.CanHover(interactable);
    }

    public override bool CanSelect(IXRSelectInteractable selectable)
    {
        if (!ValidateSelectable(selectable, this))
        {
            return false;
        }
        return base.CanSelect(selectable);
    }

    static readonly bool USELOGS = false;
    public static bool ValidateSelectable(IXRSelectInteractable selectable, XRBaseInteractor interactor)
    {
        if (interactor.firstInteractableSelected == selectable)
        {
            if(USELOGS)
                Debug.Log("Valid: currently being held", selectable as MonoBehaviour);
            return true;
        }

        if (interactor.firstInteractableSelected !=null )
        {
            if (USELOGS)
                Debug.Log("Invalid: hand occcupied", selectable as MonoBehaviour);
            return false;
        }

        if (!selectable.isSelected)
        {
            if (USELOGS)
                Debug.Log("Valid: is not selected", selectable as MonoBehaviour);
            return true;
        }

        if
        (
            selectable.interactorsSelecting.First() is IPlayerInteractor playerInteractor &&
            playerInteractor != interactor as IPlayerInteractor
        )
        {
            if (USELOGS)
                Debug.Log("Invalid: held by another hand", selectable as MonoBehaviour);
            return false;
        }

        if
        (
            selectable.firstInteractorSelecting is ExtendedSocketInteractor socketInteractor &&
            socketInteractor.HasParent ?
            (
                !socketInteractor.ParentInteractable.isSelected ||
                socketInteractor.ParentInteractable.firstInteractorSelecting is  not IPlayerInteractor
            ) : false
        )
        {
            if (USELOGS)
                Debug.Log("Invalid: attached to unheld socket with interactable", selectable as MonoBehaviour);
            return false;
        }

        return true;
    }
}

public interface IPlayerInteractor { }
public interface ICombinableInteractor 
{

}
