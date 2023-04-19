using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class JaneTesting
{
    // A Test behaves as an ordinary method
    [Test]
    public void JaneTestingSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator JaneTestingWithEnumeratorPasses()
    {
        SceneManager.LoadScene(1);
        var components = GenerateComponents();
        //do things with the components
        yield return null;
    }

    (TableAppliance, GrabItemComponent, GameStateManager.CustomerGroup)GenerateComponents()
    {
        var group = new GameStateManager.CustomerGroup();
        return (null, null, group);
    }
}
