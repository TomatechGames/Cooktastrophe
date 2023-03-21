using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineHelpers
{
    public static IEnumerator WaitUntilAll<T>(IEnumerable<T> enumerator, Func<T,bool> predicate)
    {
        foreach (var item in enumerator)
        {
            yield return new WaitUntil(()=>predicate(item));
        }
    }

    public static IEnumerator Timer(float duration, Action<float> onPercentChanged, Action onComplete)
    {
        float currentTime = 0;
        float oneOverDuration = 1/duration;
        while (currentTime<duration)
        {
            currentTime += Time.deltaTime;
            onPercentChanged(1-(currentTime*oneOverDuration));
            yield return null;
        }
        onComplete();
    }
}
