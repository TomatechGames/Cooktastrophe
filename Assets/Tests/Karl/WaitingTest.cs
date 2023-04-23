using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using static GameStateManager;

public class WaitingTest
{
    
    

    // Test 1
    // See if the Customer will enter if there is an empty table
    [UnityTest]
    public IEnumerator DoorTestSingleGroup()
    {
        yield return PrepareScene();
        var door = DoorPoint.Instance;
        GameStateManager.Instance.StartCustomDay(1, 1, 10);
        yield return null;
        Assert.IsEmpty(door.GroupList);
    }
     // Test 2
     // To see if the Customer(s) will wait outside
    [UnityTest]
    public IEnumerator DoorTestDoubleGroup()
    {
        yield return PrepareScene();
        var door = DoorPoint.Instance;
        GameStateManager.Instance.StartCustomDay(1, 1, 10);
        GameStateManager.Instance.StartCustomDay(1, 1, 10);
        yield return null;
        Assert.IsTrue(door.GroupList.Count == 1);
    }
    // This is used to load the Restuarant scene
    bool sceneIsLoaded = false;
    IEnumerator PrepareScene()
    {
        // if (sceneIsLoaded)
        // {
        //     yield break;
        // }
        SceneManager.LoadScene(1);
        yield return null;
        yield return null;
        yield return null;
        sceneIsLoaded = true;
    }
}

/*[UnityTest]
    public IEnumerator DoorTestSingleItem()
    
    {
        yield return PrepareScene();
        var door = DoorPoint.Instance;
        GameStateManager.Instance.StartCustomDay(1, 1, 10);
        yield return null;
        Assert.IsEmpty(door.GroupList);
    } */