using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;

public class BinTest
{

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator BinTestWithEnumeratorPasses()
    {
        var components = GenerateComponents();
        var binSocket = components.Item1.GetComponentInChildren<XRSocketInteractor>();
        binSocket.StartManualInteraction(components.Item2.GetComponent<XRGrabInteractable>()as IXRSelectInteractable);
        yield return new WaitForSeconds(0.1f);
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
        Assert.IsTrue(components.Item2.gameObject);
    }
    public (BinAppliance,GrabItemComponent) GenerateComponents()
    {
        var bin = Object.Instantiate(TestPrefabSource.GetPrefabWithBehavior<BinAppliance>());
        var item = Object.Instantiate(TestPrefabSource.GetPrefabWithBehavior<GrabItemComponent>());
        return(bin, item);
    }
}
