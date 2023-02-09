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
    }

    private void OnDisable()
    {
        combineLeft.action.performed -= CombineLeft;
        combineRight.action.performed -= CombineRight;
        split.action.performed -= Split;
    }

    private void CombineLeft(InputAction.CallbackContext obj) => Combine(rightHand, leftHand);

    private void CombineRight(InputAction.CallbackContext obj) => Combine(leftHand, rightHand);

    private void Combine(XRBaseControllerInteractor fromHand, XRBaseControllerInteractor toHand)
    {
        if (fromHand.interactablesSelected.Count != 1 || toHand.interactablesSelected.Count != 1)
            return;
        var movingItem = fromHand.firstInteractableSelected;

        var destinationItemBehavior = toHand.firstInteractableSelected as MonoBehaviour;
        var potentialSockets = GetSocketEndpoints(destinationItemBehavior);
        //Debug.Log(string.Join(", ",potentialSockets));

        var selectedSocket = potentialSockets.FirstOrDefault();
        if (selectedSocket)
        {
            var prevInteractor = (movingItem.firstInteractorSelecting as XRBaseInteractor);
            if(prevInteractor.isPerformingManualInteraction)
                prevInteractor.EndManualInteraction();
            selectedSocket.StartManualInteraction(movingItem);
        }
    }

    private void Split(InputAction.CallbackContext obj)
    {
        if (leftHand.interactablesSelected.Count == 1 && rightHand.interactablesSelected.Count == 0)
            Split(leftHand, rightHand);
        else if (leftHand.interactablesSelected.Count == 0 && rightHand.interactablesSelected.Count == 1)
            Split(rightHand, leftHand);
    }

    static List<XRSocketInteractor> GetSocketEndpoints(MonoBehaviour root)
    {
        List<XRSocketInteractor> result = new();
        GetSocketEndpoints(root, ref result);
        return result;
    }
    static void GetSocketEndpoints(MonoBehaviour current, ref List<XRSocketInteractor> totalEndpoints)
    {
        var currentSockets = current.GetComponentsInChildren<XRSocketInteractor>();
        foreach (var item in currentSockets)
        {
            if (item.interactablesSelected.Count == 0)
                totalEndpoints.Add(item);
            else
                GetSocketEndpoints(item.firstInteractableSelected as MonoBehaviour, ref totalEndpoints);
        }
    }

    static List<IXRSelectInteractable> GetSelectableEndpoints(MonoBehaviour root)
    {
        List<IXRSelectInteractable> result = new();
        GetSelectableEndpoints(root, ref result);
        result.Remove(root as IXRSelectInteractable);
        return result;
    }
    static void GetSelectableEndpoints(MonoBehaviour current, ref List<IXRSelectInteractable> totalEndpoints)
    {
        var currentSockets = current.GetComponentsInChildren<XRSocketInteractor>();
        bool isEndpoint = true;
        foreach (var item in currentSockets)
        {
            if (item.interactablesSelected.Count != 0)
            {
                GetSelectableEndpoints(item.firstInteractableSelected as MonoBehaviour, ref totalEndpoints);
                isEndpoint = false;
            }
        }
        if (isEndpoint)
            totalEndpoints.Add(current as IXRSelectInteractable);
    }

    private void Split(XRBaseControllerInteractor fromHand, XRBaseControllerInteractor toHand)
    {
        var sourceItemBehavior = fromHand.firstInteractableSelected as MonoBehaviour;
        var potentialEndpoints = GetSelectableEndpoints(sourceItemBehavior);

        var selectedEndpoints = potentialEndpoints.FirstOrDefault();
        if (selectedEndpoints as MonoBehaviour)
        {
            var prevInteractor = (selectedEndpoints.firstInteractorSelecting as XRBaseInteractor);
            if(prevInteractor.isPerformingManualInteraction)
                prevInteractor.EndManualInteraction();
            toHand.StartManualInteraction(selectedEndpoints);
        }
    }
}
