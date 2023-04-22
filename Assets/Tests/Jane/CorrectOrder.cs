using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using static GameStateManager;


public class CorrectOrder 
{
    const int RAW_MEAT_ID = -5073;
    const int MEDIUM_MEAT_ID = 23070;


    (TableAppliance, GrabItemComponent, GameStateManager.CustomerGroup) GenerateComponents()
    {
        var table = Object.Instantiate(TestPrefabSource.GetPrefabWithBehavior<TableAppliance>());
        var item = Object.Instantiate(TestPrefabSource.GetPrefabWithBehavior<GrabItemComponent>());
        var group = new GameStateManager.CustomerGroup();

        return (table, item, group);
    }

    
    [UnityTest]

    

   public IEnumerator CorrectOrderReceivedTest(TableAppliance tableAppliance)
    {
        var components = GenerateComponents();
        yield return null;

        components.Item2.SetNewItemID(MEDIUM_MEAT_ID);
        components.Item3.SetNewItemID(tableAppliance.customerGroup.Customers[0].GrabItemEntry.Id);

    }

  
     /* void TryDeliverFood(IXRSelectInteractable selectInteractable)
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
     */

    //private static List<CustomerController> GetCustomers() => GameStateManager.CustomerGroup.Customers;
}
