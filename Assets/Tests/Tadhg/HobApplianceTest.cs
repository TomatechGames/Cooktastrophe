using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;

public class HobApplianceTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void HobApplianceTestSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator HobApplianceTestWithEnumeratorPasses()
    {
        //doesnt depend on cooking scene any more
        //SceneManager.LoadScene(1);
        var components = GenerateComponents();
        yield return null;
        //item ID of raw meat
        var startID = -5073;
        components.Item2.SetNewItemID(startID);
        components.Item1.StartProcess(components.Item2.GetComponent<XRGrabInteractable>());

        yield return new WaitForSeconds(components.Item1.totalProcessTime);

        //item ID of rare meat
        Assert.AreEqual(2266, components.Item2.GrabItem.Id);
    }

    (HobAppliance, GrabItemComponent) GenerateComponents()
    {
        var hob = Object.Instantiate(TestPrefabSource.GetPrefabWithBehavior<HobAppliance>());
        hob.transform.position = new Vector3(0, -50, 0);
        var item = Object.Instantiate(TestPrefabSource.GetPrefabWithBehavior<GrabItemComponent>());
        item.transform.position = new Vector3(0, -50, 0);
        return (hob, item);
    }
}
