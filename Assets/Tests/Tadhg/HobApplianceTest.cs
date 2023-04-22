using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using static GameStateManager;

public class HobApplianceTest
{
    const int RAW_MEAT_ID = -5073;
    const int RARE_MEAT_ID = 2266;
    const int MEDIUM_MEAT_ID = 23070;
    const int WELL_DONE_MEAT_ID = 27442;
    // A Test behaves as an ordinary method
    //[Test]
    //public void HobApplianceTestSimplePasses()
    //{
    //    // Use the Assert class to test conditions
    //}

    // test 1
    // start process on item that can be processed once
    [UnityTest]
    public IEnumerator HobApplianceTestSingleItem()
    {
        var components = GenerateComponents();
        yield return null;

        components.Item2.SetNewItemID(MEDIUM_MEAT_ID);
        components.Item1.ItemSocket.StartManualInteraction(components.Item2.GetComponent<XRGrabInteractable>() as IXRSelectInteractable);

        yield return new WaitForSeconds(components.Item1.totalProcessTime);
        yield return null;

        Assert.AreEqual(WELL_DONE_MEAT_ID, components.Item2.GrabItem.Id);
    }

    //test 2
    //start processing item
    //interrupt process
    //start process on new item
    [UnityTest]
    public IEnumerator HobApplianceTestSingleItemAfterInterruption()
    {
        var components = GenerateComponents();
        yield return null;

        components.Item2.SetNewItemID(RAW_MEAT_ID);
        components.Item3.SetNewItemID(table.CustomerGroup.Customers[0].GrabItemEntry.Id);
        components.Item1.ItemSocket.StartManualInteraction(components.Item2.GetComponent<XRGrabInteractable>() as IXRSelectInteractable);
        yield return null;
        components.Item1.ItemSocket.EndManualInteraction();
        yield return null;
        components.Item1.ItemSocket.StartManualInteraction(components.Item3.GetComponent<XRGrabInteractable>() as IXRSelectInteractable);


        yield return new WaitForSeconds(components.Item1.totalProcessTime);
        yield return null;

        Assert.AreEqual(MEDIUM_MEAT_ID, components.Item2.GrabItem.Id);
        Assert.AreEqual(WELL_DONE_MEAT_ID, components.Item3.GrabItem.Id);
    }

    //test 3
    //start process on item without recipe
    [UnityTest]
    public IEnumerator HobApplianceTestUncookableItem()
    {
        var components = GenerateComponents();
        yield return null;

        components.Item2.SetNewItemID(WELL_DONE_MEAT_ID);
        components.Item1.ItemSocket.StartManualInteraction(components.Item2.GetComponent<XRGrabInteractable>() as IXRSelectInteractable);

        yield return new WaitForSeconds(components.Item1.totalProcessTime);
        yield return null;

        Assert.AreEqual(WELL_DONE_MEAT_ID, components.Item2.GrabItem.Id);
    }

    //test 4
    //start process on item without GrabItemComponent
    [UnityTest]
    public IEnumerator HobApplianceTestNonGrabItem()
    {
        var components = GenerateComponents();
        var basicGrabInteractable = new GameObject().AddComponent<XRGrabInteractable>() as IXRSelectInteractable;
        yield return null;

        components.Item1.ItemSocket.StartManualInteraction(basicGrabInteractable);

        // we're just making sure an error doesnt occur when trying to do this, theres nothing else to assert
    }

    //test 5
    //start process on item that can be processed 2 times
    [UnityTest]
    public IEnumerator HobApplianceTestDoubleItem()
    {
        var components = GenerateComponents();
        yield return null;

        components.Item2.SetNewItemID(RARE_MEAT_ID);
        components.Item1.ItemSocket.StartManualInteraction(components.Item2.GetComponent<XRGrabInteractable>() as IXRSelectInteractable);

        yield return new WaitForSeconds(components.Item1.totalProcessTime*2);
        yield return null;

        Assert.AreEqual(WELL_DONE_MEAT_ID, components.Item2.GrabItem.Id);
    }


    //test 6
    //start process on item that can be processed 2 times
    //wait for duration of 3 times
    [UnityTest]
    public IEnumerator HobApplianceTestDoubleItemOvertime()
    {
        var components = GenerateComponents();
        yield return null;

        components.Item2.SetNewItemID(RARE_MEAT_ID);
        components.Item1.ItemSocket.StartManualInteraction(components.Item2.GetComponent<XRGrabInteractable>() as IXRSelectInteractable);

        yield return new WaitForSeconds(components.Item1.totalProcessTime * 3);
        yield return null;

        Assert.AreEqual(WELL_DONE_MEAT_ID, components.Item2.GrabItem.Id);
    }


    (HobAppliance, GrabItemComponent, GrabItemComponent) GenerateComponents()
    {
        var hob = Object.Instantiate(TestPrefabSource.GetPrefabWithBehavior<HobAppliance>());
        var item = Object.Instantiate(TestPrefabSource.GetPrefabWithBehavior<GrabItemComponent>());
        var extraItem = Object.Instantiate(TestPrefabSource.GetPrefabWithBehavior<GrabItemComponent>());

        return (hob, item, extraItem);
    }
}
