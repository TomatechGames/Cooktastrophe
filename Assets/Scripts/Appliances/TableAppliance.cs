using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TableAppliance : MonoBehaviour
{
    [SerializeField]//this makes it drag and dropable
    XRSocketInteractor socket;
    CustomerController customerController;

    private void Start()
    {
        socket.selectEntered.AddListener(interactable => print("Hello"));
    }
    public bool TryReserveCustomer()
    {
        if (customerController)
        {
            return false;//this seat is already occupied. cannot be reserved
        }
        
       var customer =  DoorPoint.Instance.GetCustomerController();
        if (customer)
        {
            customer.ReserveSeat();
            customerController = customer;
            return true;//table is reserved
        }
        return false;//will be false if there is no one at the door
    }
}
