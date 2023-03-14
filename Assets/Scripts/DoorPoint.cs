using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPoint : MonoBehaviour
{
    static DoorPoint instance;
    //create getter
    public static   DoorPoint Instance { get { return instance; } }
    Queue<CustomerController> customerQueue = new();//creating new queue for cutsomers lined up at door

    List <TableAppliance> tables = new();

    private void Start()
    {
        instance = this;//there is only one door point, and we can get it from anywhere using Instance

    }


    public void AddCustomer(CustomerController controller)
    {

        customerQueue.Enqueue(controller);
        foreach (var item in tables)//Item is the current table were looking at. Foreach will look through all tables in "table"
        {
            if (item.TryReserveCustomer())
            {
                break;
            }
        }
    }

    public CustomerController GetCustomerController()
    {
        return customerQueue.Dequeue();
    }

}
