using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

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
        SceneManager.LoadScene(1);
        var components = GenerateComponents();
        //do things with the components
        yield return null;
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
