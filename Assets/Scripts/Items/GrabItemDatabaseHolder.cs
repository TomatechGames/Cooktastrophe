using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabItemDatabaseHolder : MonoBehaviour
{
    private static GrabItemDatabaseHolder instance;
    [SerializeField]
    GrabItemDatabase database;
    public static GrabItemDatabase Database => instance.database;
    private void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
    }
}
