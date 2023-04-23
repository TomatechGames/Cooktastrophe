using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using CustomerGroup = GameStateManager.CustomerGroup;

public class TableAppliance : MonoBehaviour, IApplianceLogic
{
    [SerializeField]//this makes it drag and dropable
    XRSocketInteractor primarySocket;
    [SerializeField]//order should match chair list
    List<XRSocketInteractor> chairSockets;
    CustomerGroup customerGroup;
    public CustomerGroup CustomerGroup => customerGroup;
    [SerializeField]
    List<Transform> chairs;
    [SerializeField]
    PopupController popupController;
    //this is super lazy, but we're low on time
    [SerializeField]
    XRGrabInteractable dirtyPlatePrefab;
    [SerializeField]
    XRBaseInteractor debugInteractor;
    ApplianceCore applianceCore;
    public ApplianceCore ApplianceCore => applianceCore ??= GetComponentInParent<ApplianceCore>();

    public Transform[] ActiveChairs => chairs.Where(c => c.gameObject.activeInHierarchy).ToArray();

    public void DebugSummon()
    {
        var interactable = debugInteractor.firstInteractableSelected;
        if(debugInteractor.isPerformingManualInteraction)
            debugInteractor.EndManualInteraction();
        primarySocket.StartManualInteraction(interactable);
    }

    private IEnumerator Start()
    {
        applianceCore = GetComponentInParent<ApplianceCore>();
        primarySocket.selectEntered.AddListener(i => StartCoroutine(CoroutineHelpers.InvokeDelayed(()=>TryDeliverFood(i.interactableObject), 0.1f)));
        primarySocket.selectExited.AddListener(i => TryReserveCustomer());
        chairSockets.ForEach(c => c.selectExited.AddListener(i => {
            TryReserveCustomer();
            c.interactionLayers &= ~InteractionLayerMask.GetMask("DirtyPlate");
        }));
        GameStateManager.Instance.OnStateChange += s => SetActive(s == GameStateManager.GameState.Dining);
        yield return null;
        /*
        applianceCore.Interactable.activated.AddListener( args =>
        {
            if (GameStateManager.Instance.CurrentState == GameStateManager.GameState.Dining && timerCoroutine != null)
                customerGroup.TakeOrder();
        });
        */
    }

    private void SetActive(bool value)
    {
        if (!value && primarySocket.socketActive)
        {
            if (primarySocket.hasSelection)
                Destroy((primarySocket.firstInteractableSelected as MonoBehaviour).gameObject);
            foreach (var chair in chairSockets)
            {
                if (chair.hasSelection)
                    Destroy((chair.firstInteractableSelected as MonoBehaviour).gameObject);
            }
        }
        primarySocket.socketActive = value;
        chairSockets.ForEach(c => c.socketActive = value);
    }

    void TryDeliverFood(IXRSelectInteractable selectInteractable)
    {
        var childSocket = selectInteractable.AsBehavior().GetComponentInChildren<XRSocketInteractor>();
        if (!childSocket || !childSocket.hasSelection || customerGroup==null)
            return; //returns null if there is no plate or the plate is empty
        if (childSocket.firstInteractableSelected.AsBehavior().TryGetComponent<GrabItemComponent>(out var grabItem))
        {
            var resultIndex = customerGroup.TryDeliverFood(grabItem.GrabItem.Id);
            if (resultIndex != -1)
            {
                if(selectInteractable.isSelected && selectInteractable.firstInteractorSelecting is XRBaseInteractor baseInteractor && baseInteractor.isPerformingManualInteraction)
                    baseInteractor.EndManualInteraction();
                (selectInteractable as XRBaseInteractable).interactionLayers = InteractionLayerMask.GetMask("UngrabbableItem");
                chairSockets[resultIndex].StartManualInteraction(selectInteractable);
            }
        }
    }

    public void ClearTable()
    {
        foreach (var socket in chairSockets)
        {
            if (!socket.hasSelection)
                continue;
            var behavior = socket.firstInteractableSelected.AsBehavior();
            Vector3 oldPos = behavior.transform.position;
            var childSocket = behavior.GetComponentInChildren<XRSocketInteractor>();
            if(childSocket && childSocket.hasSelection)
                Destroy(childSocket.firstInteractableSelected.AsBehavior().gameObject);
            Destroy(behavior.gameObject);
            Debug.Log(oldPos);
            var spawned = Instantiate(dirtyPlatePrefab);
            spawned.GetComponent<Rigidbody>().position = oldPos;
            socket.interactionLayers |= InteractionLayerMask.GetMask("DirtyPlate");
            if (socket.isPerformingManualInteraction && socket.hasSelection)
                socket.EndManualInteraction();
            socket.StartManualInteraction(spawned as IXRSelectInteractable);
        }
        customerGroup = null;
    }

    Coroutine timerCoroutine;
    public object ItemSocket;

    public void StartWaiting()
    {
        popupController.SetActive(true);
        timerCoroutine = StartCoroutine(CoroutineHelpers.Timer(GameStateManager.Instance.OrderPatience, popupController.SetPercent, GameStateManager.Instance.GameOver));
    }

    public void StopWaiting()
    {
        if (timerCoroutine != null)
        {
            customerGroup.TakeOrder();
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
            popupController.SetActive(false);
        }
    }

    public bool TryReserveCustomer()
    {
        if (customerGroup!=null)
            return false;//this table is already occupied. cannot be reserved
        if (primarySocket.hasSelection || chairSockets.FirstOrDefault(s=>s.hasSelection))
            return false;//this table still has clutter on it

        var nextGroup = DoorPoint.Instance.GetGroup(g => g.GroupSize <= ActiveChairs.Length);
        if (nextGroup!=null)
        {
            nextGroup.ReserveTable(this);
            DoorPoint.Instance.RemoveGroup(nextGroup);
            customerGroup = nextGroup;
            return true;//table is reserved
        }
        return false;//will be false if there is no one at the door
    }
}
