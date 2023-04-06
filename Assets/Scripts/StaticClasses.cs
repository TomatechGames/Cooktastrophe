using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public static class ExtensionMethods
{
    public static Vector3 Reciprocal(this Vector3 value)
    {
        return new Vector3(1 / value.x, 1 / value.y, 1 / value.z);
    }

    public static MonoBehaviour AsBehavior(this IXRSelectInteractable interactable)
    {
        return interactable as MonoBehaviour;
    }

    public static Task RunAsTask(this IEnumerator toRun) => AsyncCoroutineHelper.Instance.StartCoroutineTask(toRun);
}

public static class CoroutineHelpers
{
    public static IEnumerator InvokeDelayed(Action toRun, float delay)
    {
        yield return new WaitForSeconds(delay);
        toRun?.Invoke();
    }

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
