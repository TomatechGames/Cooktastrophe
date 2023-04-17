using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestPrefabSource : MonoBehaviour
{
    static TestPrefabSource instance;
    private void Awake()
    {
        instance = this;
    }

    [SerializeField]
    List<GameObject> prefabs;
    
    public static T GetPrefabWithBehavior<T>() where T : MonoBehaviour
    {
        return instance.prefabs.Select(b => b.GetComponent<T>()).FirstOrDefault(b => b);
    }
}
