using SerializableCallback;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ExtendedSocketInteractor : XRSocketInteractor, ICombinableInteractor
{
    List<IXRSelectInteractable> ancestorInteractables;
    IXRSelectInteractable parentInteractable;
    public IXRSelectInteractable ParentInteractable => parentInteractable;
    public bool HasParent => (parentInteractable as MonoBehaviour);
    public GameObject ParentGameObject => (parentInteractable as MonoBehaviour).gameObject;

    public IXRSelectInteractable RootInteractable => ancestorInteractables.FirstOrDefault();

    protected override void Awake()
    {
        base.Awake();
        parentInteractable = GetComponentInParent<IXRSelectInteractable>();
        UpdateAncestorList();
    }

    protected override void Start()
    {
        base.Start();
        if (!HasParent)
        {
            ActivateSlot();
            return;
        }
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

    public void DeactivateSlot(SelectExitEventArgs args = null)
    {
        allowNewSelections = false;
        if (hasSelection && (firstInteractableSelected as MonoBehaviour).TryGetComponent<ExtendedSocketInteractor>(out var childSocket))
        {
            childSocket.DeactivateSlot();
            //interactablesSelected.First().colliders.ForEach(c => c.enabled = false);
        }
        UpdateAncestorList();
    }
    public void ActivateSlot(SelectEnterEventArgs args = null)
    {
        allowNewSelections = true;
        if (hasSelection && (firstInteractableSelected as MonoBehaviour).TryGetComponent<ExtendedSocketInteractor>(out var childSocket))
        {
            childSocket.ActivateSlot();
            //interactablesSelected.First().colliders.ForEach(c => c.enabled = true);
        }
        UpdateAncestorList();
    }

    public override bool CanHover(IXRHoverInteractable interactable)
    {
        if (allowNewSelections && interactable is IXRSelectInteractable selectable && ValidateSelectable(selectable) && !ancestorInteractables.Contains(selectable))
        {
            return base.CanHover(interactable);
        }
        return false;
    }

    public override bool CanSelect(IXRSelectInteractable selectable)
    {
        bool result = (IsSelecting(selectable) && selectable.interactorsSelecting.Count == 1) ||
            (selectable == attemptedSelectInteractable && allowNewSelections && interactablesSelected.Count==0 && !ancestorInteractables.Contains(selectable));
        attemptedSelectInteractable = null;
        return result;
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);

        //improve this for stack sub-children

        if ((args.interactableObject as MonoBehaviour).TryGetComponent<ExtendedSocketInteractor>(out var childSocket))
            if (allowNewSelections)
                childSocket.ActivateSlot();
            else
                childSocket.DeactivateSlot();

        if(HasParent)
            SetInteractableIgnorance(args.interactableObject, parentInteractable, true);
        //args.interactableObject.colliders.ForEach(c => c.isTrigger = true);
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);
        if ((args.interactableObject as MonoBehaviour).TryGetComponent<ExtendedSocketInteractor>(out var childSocket))
            childSocket.DeactivateSlot();
        if (HasParent)
            SetInteractableIgnorance(args.interactableObject, parentInteractable, false);
        //SetInteractableIgnorance(args.interactableObject, parentInteractable, false);
        //args.interactableObject.colliders.ForEach(c => c.isTrigger = false);
    }

    protected void OnGrabSelectEntered(SelectEnterEventArgs args)
    {
    }

    protected void OnGrabSelectExited(SelectExitEventArgs args)
    {
    }

    void UpdateAncestorList()
    {
        ancestorInteractables = GetAncestorList();
        //Debug.Log(string.Join(", ", ancestorInteractables));
    }

    List<IXRSelectInteractable> GetAncestorList()
    {
        if (!HasParent)
            return new();
        if (parentInteractable.isSelected && (parentInteractable.firstInteractorSelecting as MonoBehaviour).TryGetComponent<ExtendedSocketInteractor>(out var parentSocket) && parentSocket!=this)
        {
            var parentAncestors = parentSocket.GetAncestorList().ToList();
            parentAncestors.Add(parentInteractable);
            return parentAncestors;
        }
        else
            return new() { parentInteractable };
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
        //if (attemptedSelectInteractable.isSelected && attemptedSelectInteractable.firstInteractorSelecting!=this as IXRSelectInteractable)
        //{
        //    var prevInteractor = (attemptedSelectInteractable.firstInteractorSelecting as XRBaseInteractor);
        //    Debug.Log(prevInteractor);
        //    if(prevInteractor.isPerformingManualInteraction)
        //        prevInteractor.EndManualInteraction();
        //}
        if(interactablesSelected.Count==0 && !attemptedSelectInteractable.isSelected)
            StartManualInteraction(attemptedSelectInteractable);
    }

    bool ValidateSelectable(IXRSelectInteractable selectable)
    {
        return  selectable.isSelected &&
        selectable.interactorsSelecting
            .Where(i => i is IPlayerInteractor)
            .Count() > 0;
    }
}
