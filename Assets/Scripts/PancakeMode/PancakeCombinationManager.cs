using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PancakeCombinationManager : MonoBehaviour
{
    [SerializeField]
    InputActionReference combineLeft;
    [SerializeField]
    InputActionReference combineRight;
    [SerializeField]
    InputActionReference split;
    [SerializeField]
    InputActionReference process;

    [Space]
    [SerializeField]
    XRBaseControllerInteractor leftHand;
    [SerializeField]
    XRBaseControllerInteractor rightHand;
    [SerializeField]
    ExtendedSocketInteractor testSocket;

    private IEnumerator Start()
    {
        yield return null;
        //retrieveHands
    }

    private void OnEnable()
    {
        combineLeft.action.performed += CombineLeft;
        combineRight.action.performed += CombineRight;
        split.action.performed += Split;
        process.action.performed += Process;
    }

    private void OnDisable()
    {
        combineLeft.action.performed -= CombineLeft;
        combineRight.action.performed -= CombineRight;
        split.action.performed -= Split;
        process.action.performed -= Process;
    }

    private void CombineLeft(InputAction.CallbackContext obj) => Combine(rightHand, leftHand);

    private void CombineRight(InputAction.CallbackContext obj) => Combine(leftHand, rightHand);

    private void Combine(XRBaseControllerInteractor fromHand, XRBaseControllerInteractor toHand)
    {
        if (fromHand.interactablesSelected.Count != 1 || toHand.interactablesSelected.Count != 1)
            return;

        var movingItem = fromHand.firstInteractableSelected;
        var destinationItem = toHand.firstInteractableSelected;
        var destinationItemBehavior = destinationItem as MonoBehaviour;

        var selectedSocket = GrabItemUtils.GetSocketEndpoints(destinationItemBehavior, true).FirstOrDefault();
        bool hasCombiners = GrabItemComponent.TryRemap(destinationItem, out var destGrabComponent);
        if (selectedSocket)
            hasCombiners= GrabItemComponent.TryRemap(selectedSocket.firstInteractableSelected, out destGrabComponent);
        hasCombiners &= GrabItemComponent.TryRemap(movingItem, out var sourceGrabComponent);
        if (hasCombiners && GrabItemUtils.TryCombine(sourceGrabComponent, destGrabComponent))
            return;

        var emptySocket = GrabItemUtils.GetSocketEndpoints(destinationItemBehavior).FirstOrDefault();
        if (emptySocket)
        {
            var prevInteractor = (movingItem.firstInteractorSelecting as XRBaseInteractor);
            if(prevInteractor.isPerformingManualInteraction)
                prevInteractor.EndManualInteraction();
            emptySocket.StartManualInteraction(movingItem);
        }
    }

    private void Process(InputAction.CallbackContext obj)
    {
        Process(leftHand);
        Process(rightHand);
    }

    private void Process(XRBaseControllerInteractor targetHand)
    {
        var itemA = targetHand.firstInteractableSelected as MonoBehaviour;

        if (!itemA)
            return;
        var validatedA = itemA.TryGetComponent<GrabItemComponent>(out var sourceGrabItem);
        if (!validatedA)
            return;

        var recipe = GrabItemDatabaseHolder.Database.GetProcessEntry(sourceGrabItem.GrabItem.Id, ProcessType.Heat);
        if (recipe == null)
            return;

        sourceGrabItem.SetNewItemID(recipe.Result);
    }

    private void Split(InputAction.CallbackContext obj)
    {
        if (leftHand.interactablesSelected.Count == 1 && rightHand.interactablesSelected.Count == 0)
            Split(leftHand, rightHand);
        else if (leftHand.interactablesSelected.Count == 0 && rightHand.interactablesSelected.Count == 1)
            Split(rightHand, leftHand);
    }


    private void Split(XRBaseControllerInteractor fromHand, XRBaseControllerInteractor toHand)
    {
        var sourceItemBehavior = fromHand.firstInteractableSelected as MonoBehaviour;
        var potentialEndpoints = GrabItemUtils.GetSelectableEndpoints(sourceItemBehavior);

        var selectedEndpoints = potentialEndpoints.FirstOrDefault();
        if (selectedEndpoints as MonoBehaviour)
        {
            var prevInteractor = (selectedEndpoints.firstInteractorSelecting as XRBaseInteractor);
            if (prevInteractor.isPerformingManualInteraction)
                prevInteractor.EndManualInteraction();
            toHand.StartManualInteraction(selectedEndpoints);
        }
    }

}
