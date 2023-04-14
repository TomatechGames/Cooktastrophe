using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
            shouldBeSelected = !(
                    interactor.firstInteractableSelected is not XRGrabInteractable
                    ||
                    (
                        selectPressed &&
                        !selectBuffer &&
                        (interactor.firstInteractableSelected as IXRHoverInteractable).interactorsHovering.Exists(i=>i is XRSocketInteractor socket && !socket.hasSelection)
                    )
                 );

            if (selectPressed && !selectBuffer && interactor is ExtendedRayInteractor rayInteractor)
            {
                var hits = Physics.RaycastAll(rayInteractor.rayOriginTransform.position, rayInteractor.rayOriginTransform.forward, rayInteractor.endPointDistance);

                //var hitSocket = hits.FirstOrDefault(h=> h.collider && h.collider.TryGetComponent<XRSocketInteractor>(out var socket) && socket.socketActive && socket.interactablesSelected.Count == 0 && (socket.interactionLayers.value & interactor.firstInteractableSelected.interactionLayers.value)!=0 && socket.CanHover(interactor.firstInteractableSelected as IXRHoverInteractable));
                RaycastHit hitSocket = default;

                foreach (var hit in hits)
                {
                    if(hit.collider && hit.collider.TryGetComponent<XRSocketInteractor>(out var socket))
                    {
                        if (Input.GetKey(KeyCode.L))
                            Debug.Log("Exists and has socket");
                        if (socket.socketActive && socket.interactablesSelected.Count == 0)
                        {
                            if (Input.GetKey(KeyCode.L))
                                Debug.Log("Socket active and empty");
                            if ((socket.interactionLayers.value & interactor.firstInteractableSelected.interactionLayers.value) != 0)
                            {
                                if (Input.GetKey(KeyCode.L))
                                    Debug.Log("Layers have intersection");
                                if (socket.CanHover(interactor.firstInteractableSelected as IXRHoverInteractable))
                                {
                                    if (Input.GetKey(KeyCode.L))
                                        Debug.Log("Can Hover");
                                    hitSocket = hit;
                                    break;
                                }
                            }
                        }
                    }
                }

                Debug.Log(hits.FirstOrDefault().collider.gameObject, hits.FirstOrDefault().collider.gameObject);
                if (hitSocket.collider)
                {
                    var socket = hitSocket.collider.GetComponent<XRSocketInteractor>();
                    var target = interactor.firstInteractableSelected;
                    if(interactor.isPerformingManualInteraction)
                        interactor.EndManualInteraction();
                    Debug.Log(socket, socket.gameObject);
                    Debug.Log(target, (target as MonoBehaviour).gameObject);
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
