using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using CustomerGroup = GameStateManager.CustomerGroup;

public class TableAppliance : MonoBehaviour
{
    [SerializeField]//this makes it drag and dropable
    XRSocketInteractor socket;
    CustomerGroup customerGroup;
    [SerializeField]
    List<Transform> chairs;
    public List<Transform> ActiveChairs => chairs.Where(c=>c.gameObject.activeInHierarchy).ToList();
    [SerializeField]
    PopupController popupController;

    private void Start()
    {
        socket.selectEntered.AddListener(interactable => print("Hello"));
    }

    Coroutine timerCoroutine;

    public void StartWaiting()
    {
        popupController.SetActive(true);
        timerCoroutine = StartCoroutine(CoroutineHelpers.Timer(GameStateManager.Instance.OrderPatience, popupController.SetPercent, GameStateManager.Instance.GameOver));
    }

    public void StopWaiting()
    {
        StopCoroutine(timerCoroutine);
        popupController.SetActive(false);
    }

    public bool TryReserveCustomer()
    {
        if (customerGroup!=null)
            return false;//this seat is already occupied. cannot be reserved

        var nextGroup = DoorPoint.Instance.GetGroup(g => g.GroupSize <= ActiveChairs.Count);
        if (nextGroup!=null)
        {
            nextGroup.ReserveTable(this);
            customerGroup = nextGroup;
            return true;//table is reserved
        }
        return false;//will be false if there is no one at the door
    }
}
