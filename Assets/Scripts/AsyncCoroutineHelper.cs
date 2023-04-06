using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncCoroutineHelper : MonoBehaviour
{
    static AsyncCoroutineHelper instance;
    public static AsyncCoroutineHelper Instance
    {
        get
        {
            if (!instance)
            {
                instance = new GameObject().AddComponent<AsyncCoroutineHelper>();
                DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }

    public async Task StartCoroutineTask(IEnumerator routine)
    {
        ResultContainer result = new();
        StartCoroutine(RunEnumerator(result, routine)); 
        while (!result.isDone)
        {
            await Task.Delay(25);
        }
    }

    IEnumerator RunEnumerator(ResultContainer resultObj, IEnumerator routine)
    {
        yield return routine;
        resultObj.isDone = true;
    }

    class ResultContainer
    {
        public bool isDone = false;
    }
}
