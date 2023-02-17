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
        if (TryUseRecipe(fromHand, toHand))
            return;

        var movingItem = fromHand.firstInteractableSelected;
        var destinationItemBehavior = toHand.firstInteractableSelected as MonoBehaviour;
        //Debug.Log(string.Join(", ",potentialSockets));

        var selectedSocket = GetSocketEndpoints(destinationItemBehavior).FirstOrDefault();
        if (selectedSocket)
        {
            var prevInteractor = (movingItem.firstInteractorSelecting as XRBaseInteractor);
            if(prevInteractor.isPerformingManualInteraction)
                prevInteractor.EndManualInteraction();
            selectedSocket.StartManualInteraction(movingItem);
        }
    }

    //TODO: move to relevant class
    private bool TryUseRecipe(XRBaseControllerInteractor fromHand, XRBaseControllerInteractor toHand)
    {
        var itemA = fromHand.firstInteractableSelected as MonoBehaviour;

        if (!itemA)
            return false;
        var validatedA = itemA.TryGetComponent<GrabItemComponent>(out var sourceGrabItem);
        if(!validatedA)
            return false;

        var itemB = toHand.firstInteractableSelected as MonoBehaviour;

        var selectedSocket = GetSocketEndpoints(itemB, true).FirstOrDefault();
        if (selectedSocket)
            itemB = selectedSocket.firstInteractableSelected as MonoBehaviour;

        if(!itemB)
            return false;
        var validatedB = itemA.TryGetComponent<GrabItemComponent>(out var destGrabItem);
        if (!validatedB)
            return false;

        var recipe = GrabItemDatabaseHolder.Database.GetCombinationEntry(sourceGrabItem.GrabItem.Id, destGrabItem.GrabItem.Id );
        if (recipe == null) 
            recipe = GrabItemDatabaseHolder.Database.GetCombinationEntry(destGrabItem.GrabItem.Id, sourceGrabItem.GrabItem.Id);

        if (recipe == null)
            return false;

        destGrabItem.SetNewItemID(recipe.Result);
        Destroy(itemA.gameObject);

        return true;
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
        var potentialEndpoints = GetSelectableEndpoints(sourceItemBehavior);

        var selectedEndpoints = potentialEndpoints.FirstOrDefault();
        if (selectedEndpoints as MonoBehaviour)
        {
            var prevInteractor = (selectedEndpoints.firstInteractorSelecting as XRBaseInteractor);
            if (prevInteractor.isPerformingManualInteraction)
                prevInteractor.EndManualInteraction();
            toHand.StartManualInteraction(selectedEndpoints);
        }
    }

    static List<XRSocketInteractor> GetSocketEndpoints(MonoBehaviour root, bool withChild=false)
    {
        List<XRSocketInteractor> result = new();
        GetSocketEndpoints(root, ref result, withChild);
        return result;
    }
    static void GetSocketEndpoints(MonoBehaviour current, ref List<XRSocketInteractor> totalEndpoints, bool withChild)
    {
        var currentSockets = current.GetComponentsInChildren<XRSocketInteractor>().ToList();
        foreach (var item in currentSockets)
        {
            if (withChild == (item.interactablesSelected.Count != 0))
                totalEndpoints.Add(item);
            if(item.interactablesSelected.Count != 0)
                GetSocketEndpoints(item.firstInteractableSelected as MonoBehaviour, ref totalEndpoints, withChild);
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
}
