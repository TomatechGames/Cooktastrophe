using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;

public class FridgeApplianceTest
{
    // A Test behaves as an ordinary method

    

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator FridgeApplianceTestWithEnumeratorPasses()
    {
        yield return PrepareScene();
        yield return null;
        var fridge = Object.FindObjectOfType<FridgeAplliance>();
        GameStateManager.Instance.StartDay();
        yield return null;
        var fridgeSocket = fridge.GetComponentInChildren<XRSocketInteractor>();
        Assert.IsTrue(fridgeSocket.hasSelection);
        fridgeSocket.EndManualInteraction();
        yield return 2;
        Assert.IsTrue(fridgeSocket.hasSelection);
    }
    bool sceneIsLoaded = false;
    IEnumerator PrepareScene()
    {
        if (sceneIsLoaded)
        {
            yield break;
        }
        SceneManager.LoadScene(1);
        yield return null;
        yield return null;
        yield return null;
        sceneIsLoaded = true;
    }
}
