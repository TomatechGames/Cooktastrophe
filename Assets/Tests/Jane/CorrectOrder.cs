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
    public IEnumerator FoodPlacedNoGroup()
    {
        yield return PrepareScene();
        yield return null;

        var table = Object.FindObjectOfType<TableAppliance>();//finds the table and puts it into the variable
        //Try to put some food onto the table while the day hasnt been started, meaning no groups are linked to the table
        var item = Object.Instantiate(TestPrefabSource.GetPrefabWithBehavior<GrabItemComponent>());
        table.PrimarySocket.StartManualInteraction(item.GetComponent<XRGrabInteractable>()as IXRSelectInteractable);
        yield return null;
        Assert.IsTrue(table.PrimarySocket.hasSelection);//The food should not move anywhere
        //This test ensures that no errors occur when an item is placed on the table with no customer group



        IEnumerator PrepareScene()
        {
            
            SceneManager.LoadScene(1);
            yield return null;
            yield return null;
            yield return null;


        }
    }

    [UnityTest]
    public IEnumerator CorrectFoodPlacedWithGroup()//second test
    {
        yield return PrepareScene();
        yield return null;

        var table = Object.FindObjectOfType<TableAppliance>();//finds the table and puts it into the variable
        //Try to put some food onto the table while the day hasnt been started, meaning no groups are linked to the table
        var item = Object.Instantiate(TestPrefabSource.GetPrefabWithBehavior<GrabItemComponent>());
        GameStateManager.Instance.StartDay();
        GameStateManager.TestingSkipPathfinding = true;
        yield return null;
        table.StopWaiting();
        yield return null;
        item.SetNewItemID(table.CustomerGroup.Customers[0].GrabItemEntry.Id);//Garuntees we are picking the ID of the food that the customer wants
        table.PrimarySocket.StartManualInteraction(item.GetComponent<XRGrabInteractable>() as IXRSelectInteractable);
        yield return null;
        Assert.IsFalse(table.PrimarySocket.hasSelection);
        //This test ensures that no errors occur when an item is placed on the table with no customer group



        IEnumerator PrepareScene()
        {

            SceneManager.LoadScene(1);
            yield return null;
            yield return null;
            yield return null;


        }
    }


    /* void TryDeliverFood(IXRSelectInteractable selectInteractable)
   {
       var childSocket = selectInteractable.AsBehavior().GetComponentInChildren<XRSocketInteractor>();
       if (!childSocket || !childSocket.hasSelection || customerGroup==null)
           return; //returns null if there is no plate or the plate is empty //first test
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
