using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;

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
    public IEnumerator HobApplianceTestWithEnumeratorPasses()
    {
        //doesnt depend on cooking scene any more
        //SceneManager.LoadScene(1);
        var components = GenerateComponents();
        yield return null;

        components.Item2.SetNewItemID(RAW_MEAT_ID);
        components.Item1.StartProcess(components.Item2.GetComponent<XRGrabInteractable>());

        yield return new WaitForSeconds(components.Item1.totalProcessTime);
        yield return null;

        //item ID of rare meat
        Assert.AreEqual(RARE_MEAT_ID, components.Item2.GrabItem.Id);
    }

    //test 2
    //start processing item
    //interrupt process
    //start process on new item

    //test 3
    //start processing item with no process recipe

    //test 4
    //start processing interactable with no grabitemcomponent

    //test 5
    //start process on item that can be processed 2 times


    //test 6
    //start process on item that can be processed 2 times
    // wait the equivelent of 3 process lengths


    (HobAppliance, GrabItemComponent, GrabItemComponent) GenerateComponents()
    {
        var hob = Object.Instantiate(TestPrefabSource.GetPrefabWithBehavior<HobAppliance>());
        var item = Object.Instantiate(TestPrefabSource.GetPrefabWithBehavior<GrabItemComponent>());
        var extraItem = Object.Instantiate(TestPrefabSource.GetPrefabWithBehavior<GrabItemComponent>());

        return (hob, item, extraItem);
    }
}
