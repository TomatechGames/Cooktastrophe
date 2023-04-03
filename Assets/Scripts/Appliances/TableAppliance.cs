using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEditor.Progress;
using CustomerGroup = GameStateManager.CustomerGroup;

public class TableAppliance : MonoBehaviour, IApplianceLogic
{
    [SerializeField]//this makes it drag and dropable
    XRSocketInteractor primarySocket;
    [SerializeField]//order should match chair list
    List<XRSocketInteractor> chairSockets;
    CustomerGroup customerGroup;
    [SerializeField]
    List<Transform> chairs;
    [SerializeField]
    PopupController popupController;
    ApplianceCore applianceCore;
    [SerializeField]
    XRBaseInteractor debugInteractor;
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
        primarySocket.selectEntered.AddListener(i => TryReserveCustomer());
        primarySocket.selectExited.AddListener(i => TryReserveCustomer());
        chairSockets.ForEach(c => c.selectExited.AddListener(i => TryReserveCustomer()));
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

    void TryDeliverFood(IXRSelectInteractable selectInteractable)
    {
        if (selectInteractable is MonoBehaviour interactableBehavior && interactableBehavior.TryGetComponent<GrabItemComponent>(out var grabItemInteractable))
        {
            for (int i = 0; i < customerGroup.GroupSize; i++)
            {
                
            }
        }
    }

    private void SetActive(bool value)
    {
        if (!value && primarySocket.socketActive)
        {
            if (primarySocket.hasSelection)
                Destroy((primarySocket.firstInteractableSelected as MonoBehaviour).gameObject);
            foreach (var chair in chairSockets)
            {
                if(chair.hasSelection)
                    Destroy((chair.firstInteractableSelected as MonoBehaviour).gameObject);
            }
        }
        primarySocket.socketActive = value;
        chairSockets.ForEach(c => c.socketActive = value);
    }

    Coroutine timerCoroutine;

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

    public void FreeTable()
    {
        customerGroup = null;
    }
}
