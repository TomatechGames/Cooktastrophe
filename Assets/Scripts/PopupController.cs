using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    Transform cachedTarget;
    [SerializeField]
    Transform canvas;
    [SerializeField]
    Image percentImage;
    [SerializeField]
    bool isActive;

    private void Start()
    {
        UpdateActive();
        if (Camera.main)
            cachedTarget = Camera.main.transform;
    }

    public void SetPercent(float percent)
    {
        percentImage.fillAmount = percent;
    }

    public void SetActive(bool isActive)
    {
        if (this.isActive == isActive)
            return;
        this.isActive = isActive;

        UpdateActive();
    }

    void UpdateActive()
    {
        if (isActive)
        {
            transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
        }
        else
        {
            transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
        }
    }

    private void LateUpdate()
    {
        if (!cachedTarget)
            return;
        canvas.LookAt(cachedTarget);
    }
}
