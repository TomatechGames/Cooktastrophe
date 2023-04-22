using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;

public class ApplianceGrabbabilityTest
{
    //this probably isnt how one should unit test
    [UnityTest]
    public IEnumerator ApplianceGrabbabilityTestWithEnumeratorPasses()
    {
        yield return PrepareScene();
        var someAppliance = Object.FindObjectOfType<ApplianceCore>();
        //checks if the interactable has the Appliance layer, allowing it to be grabbable
        Assert.IsTrue((someAppliance.Interactable.interactionLayers & InteractionLayerMask.GetMask("Appliance")) != 0);
        GameStateManager.Instance.StartDay();
        //checks if the interactable does not have the Appliance layer, preventing it from being grabbable
        Assert.IsFalse((someAppliance.Interactable.interactionLayers & InteractionLayerMask.GetMask("Appliance")) != 0);
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
