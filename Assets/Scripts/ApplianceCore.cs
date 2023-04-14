using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ApplianceCore : MonoBehaviour
{
    //shrink into blueprint when grabbed
    //prevent grab in dining mode (actually, do this in the interactor using filters)
    //interact rotates in renovation mode
    List<Collider> colliders;
    XRGrabInteractable interactable;
    [SerializeField]
    float height = 1;
    [SerializeField]
    Transform box;
    [SerializeField]
    Transform yawParent;
    [SerializeField]
    Transform pitchScaleParent;
    [SerializeField]
    Transform flattenParent;

    public XRGrabInteractable Interactable => interactable;

    public bool IsSlotted => interactable.isSelected;

    private void Start()
    {
        colliders = GetComponentsInChildren<Collider>().ToList();
        colliders.Remove(GetComponent<SphereCollider>());
        interactable = GetComponent<XRGrabInteractable>();
        interactable.selectEntered.AddListener(Flatten);
        interactable.selectExited.AddListener(Unflatten);
        GameStateManager.Instance.OnStateChange += s => SetSolid(s==GameStateManager.GameState.Dining);
    }

    bool boxModeActive = false;

    public void SetSolid(bool value)
    {
        interactable.interactionLayers = value ? InteractionLayerMask.GetMask() : InteractionLayerMask.GetMask("Appliance");
        if(boxModeActive && gameObject)
        {
            Destroy(gameObject);
        }
    }

    public void EnterBox()
    {
        box.localScale = Vector3.one;
        yawParent.localRotation = Quaternion.Euler(0, 45, 0);
        pitchScaleParent.localRotation = Quaternion.Euler(45, 0, 0);
        pitchScaleParent.localScale = height * 0.2f * Vector3.one;
        flattenParent.localScale = new (1, 0.01f, 1);
        flattenParent.localRotation = Quaternion.Euler(0, 0, 0);
        boxModeActive = true;
    }

    void Flatten(SelectEnterEventArgs e)
    {
        if (e.interactorObject is not IPlayerInteractor)
            return;
        colliders.ForEach(c=>c.enabled=false);
        yawParent.DOLocalRotate(new(0, 45, 0), 0.1f).SetEase(Ease.OutQuad);
        pitchScaleParent.DOLocalRotate(new(45, 0, 0), 0.1f).SetEase(Ease.OutQuad);
        pitchScaleParent.DOScale(height * 0.2f * Vector3.one, 0.1f).SetEase(Ease.OutQuad);
        flattenParent.DOScale(new Vector3(1, 0.01f, 1), 0.1f).SetEase(Ease.OutQuad);
        flattenParent.DOLocalRotate(new(-90, 0, 0), 0.1f).SetEase(Ease.OutQuad);
        box.DOScale(0, 0.1f).SetEase(Ease.OutQuad);
        boxModeActive = false;
    }

    void Unflatten(SelectExitEventArgs e)
    {
        if (e.interactorObject is not IPlayerInteractor)
            return;
        yawParent.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.InQuad);
        pitchScaleParent.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.InQuad);
        pitchScaleParent.DOScale(Vector3.one, 0.1f).SetEase(Ease.InQuad);
        flattenParent.DOScale(Vector3.one, 0.1f).SetEase(Ease.InQuad).onComplete = () => colliders.ForEach(c => c.enabled = true);
        flattenParent.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.OutQuad);
    }
}

public interface IApplianceLogic
{
    public ApplianceCore ApplianceCore { get; }
}
