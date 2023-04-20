using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestPrefabSource : MonoBehaviour
{
    static TestPrefabSource instance;
    static TestPrefabSource Instance
    {
        get {
            if (!instance)
            {
                instance = Instantiate(Resources.Load("TestPrefabSource") as GameObject).GetComponent<TestPrefabSource>();
            }
            return instance; 
        }
    }

    [SerializeField]
    List<GameObject> prefabs;
    
    public static T GetPrefabWithBehavior<T>() where T : MonoBehaviour
    {
        return Instance.prefabs.Select(b => b.GetComponent<T>()).FirstOrDefault(b => b);
    }
}
