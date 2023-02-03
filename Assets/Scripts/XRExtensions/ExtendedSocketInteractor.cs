using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ExtendedSocketInteractor : XRSocketInteractor
{
    [SerializeField]
    IXRSelectInteractable parentInteractable;
    public IXRSelectInteractable ParentInteractable => parentInteractable;
    public bool HasParent => (parentInteractable as MonoBehaviour);
    public GameObject ParentGameObject => (parentInteractable as MonoBehaviour).gameObject;

    protected override void Awake()
    {
        base.Awake();
        parentInteractable = GetComponentInParent<IXRSelectInteractable>();
    }

    protected override void Start()
    {
        base.Start();
        if (!HasParent)
            return;
        if (parentInteractable.isSelected)
            ActivateSlot();
        else
            DeactivateSlot();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!HasParent)
            return;
        parentInteractable.selectEntered.AddListener(ActivateSlot);
        parentInteractable.selectExited.AddListener(DeactivateSlot);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (!HasParent)
            return;
        parentInteractable.selectEntered.RemoveListener(ActivateSlot);
        parentInteractable.selectExited.RemoveListener(DeactivateSlot);
    }

    bool allowNewSelections = false;

    void DeactivateSlot(SelectExitEventArgs args = null)
    {
        allowNewSelections = false;
        if (hasSelection)
        {
            //interactablesSelected.First().colliders.ForEach(c => c.enabled = false);
        }
    }
    void ActivateSlot(SelectEnterEventArgs args = null)
    {
        allowNewSelections = true;
        if (hasSelection)
        {
            //interactablesSelected.First().colliders.ForEach(c => c.enabled = true);
        }
    }

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        if (allowNewSelections && interactable is IXRSelectInteractable selectable && ValidateSelectable(selectable))
        {
            return base.CanHover(interactable);
        }
        return false;
    }

    public override bool CanSelect(IXRSelectInteractable selectable)
    {
        bool result = (IsSelecting(selectable) && selectable.interactorsSelecting.Count == 1) ||
            (selectable == attemptedSelectInteractable && allowNewSelections && interactablesSelected.Count==0);
        attemptedSelectInteractable = null;
        return result;
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        if(HasParent)
            SetInteractableIgnorance(args.interactableObject, parentInteractable, true);
        //args.interactableObject.colliders.ForEach(c => c.isTrigger = true);
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);
        if (HasParent)
            SetInteractableIgnorance(args.interactableObject, parentInteractable, false);
        //SetInteractableIgnorance(args.interactableObject, parentInteractable, false);
        //args.interactableObject.colliders.ForEach(c => c.isTrigger = false);
    }

    static void SetInteractableIgnorance(IXRSelectInteractable objA, IXRSelectInteractable objB, bool ignored)
    {
        foreach (var a in objA.colliders)
        {
            foreach (var b in objB.colliders)
            {
                if (a == b)
                    continue;
                Physics.IgnoreCollision(a, b, ignored);
            }
        }
    }

    protected override void OnHoverEntering(HoverEnterEventArgs args)
    {
        if (args.interactableObject is IXRSelectInteractable selectable)
            selectable.selectExited.AddListener(PerformSelect);
        base.OnHoverEntering(args);
    }

    protected override void OnHoverExiting(HoverExitEventArgs args)
    {
        if (args.interactableObject is IXRSelectInteractable selectable)
            selectable.selectExited.RemoveListener(PerformSelect);
        base.OnHoverExiting(args);
    }

    IXRSelectInteractable attemptedSelectInteractable;
    void PerformSelect(SelectExitEventArgs args)
    {
        attemptedSelectInteractable = args.interactableObject;
        interactionManager.SelectEnter(this, args.interactableObject);
        //attemptedSelectInteractable = null;
    }

    bool ValidateSelectable(IXRSelectInteractable selectable)
    {
        return  selectable.isSelected &&
        selectable.interactorsSelecting
            .Where(i => i is IPlayerInteractor)
            .Count() > 0;
    }
}

public interface IPlayerInteractor { }
