using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//This extension makes Selections operate on a toggle. Pressing the grip button with no object selected will try to select the object, otherwise it will deselect the current object
public class ExtendedActionBasedController : ActionBasedController
{
    [SerializeField]
    XRBaseInteractor interactor;
    [SerializeField]
    bool selectBuffer;

    protected override void UpdateInput(XRControllerState controllerState)
    {
        base.UpdateInput(controllerState);
        if (controllerState == null || !interactor)
            return;

        controllerState.selectInteractionState.ResetFrameDependent();
        var selectValueAction = selectActionValue.action;
        if (selectValueAction == null || selectValueAction.bindings.Count <= 0)
            selectValueAction = selectAction.action;

        bool selectPressed = IsPressed(selectAction.action);
        bool shouldBeSelected;

        if (interactor.hasSelection)
        {
            //Debug.Log("hasSelection");
            shouldBeSelected = !(
                selectPressed && 
                !selectBuffer && 
                interactor.firstInteractableSelected is IXRHoverInteractable hoverable && 
                hoverable.interactorsHovering
                    .Exists(i=>i is XRSocketInteractor socket && !socket.hasSelection));

            if(selectPressed && !selectBuffer && interactor is ExtendedRayInteractor rayInteractor)
            {
                //Debug.Log("Welp");
                var hits = Physics.RaycastAll(rayInteractor.rayOriginTransform.position, rayInteractor.rayOriginTransform.forward, 5);
                var hitSocket = hits.FirstOrDefault(h=> h.collider && h.collider.TryGetComponent<XRSocketInteractor>(out var socket) && socket.interactablesSelected.Count == 0);

                if (hitSocket.collider)
                {
                    var socket = hitSocket.collider.GetComponent<XRSocketInteractor>();
                    //Debug.Log("socket Found");
                    var target = interactor.firstInteractableSelected;
                    if(interactor.isPerformingManualInteraction)
                        interactor.EndManualInteraction();
                    socket.StartManualInteraction(target);
                    shouldBeSelected = false;
                }
            }
        }
        else
        {
            shouldBeSelected = (selectPressed && !selectBuffer);
        }

        controllerState.selectInteractionState.SetFrameState(shouldBeSelected, ReadValue(selectValueAction));

        selectBuffer = selectPressed;
    }
}
