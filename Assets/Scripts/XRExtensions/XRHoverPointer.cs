using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHoverPointer : MonoBehaviour
{
    [SerializeField]
    XRBaseInteractor interactor;
    [SerializeField]
    GameObject appearance;
    [SerializeField]
    Transform currentTarget;
    [SerializeField]
    bool lookAtObject;
    [SerializeField]
    float shrinkAtDist = 0.05f;

    private void Start()
    {
        UpdateHover();
        interactor.hoverEntered.AddListener(e=>UpdateHover());
        interactor.hoverExited.AddListener(e => UpdateHover());
    }

    public void UpdateHover()
    {
        if (!lookAtObject)
            transform.rotation = Quaternion.Euler(-90,0,0);
        if (!interactor.hasHover)
            currentTarget = null;
        else
            currentTarget = interactor.interactablesHovered[0].transform;
        appearance.SetActive(currentTarget);
    }

    private void LateUpdate()
    {
        if (lookAtObject && currentTarget)
        {
            transform.position = currentTarget.position;
            transform.LookAt(transform.parent);
            float dist = (transform.parent.position - transform.position).magnitude;
            if (dist < shrinkAtDist)
                transform.localScale = Vector3.one * (dist / shrinkAtDist);
            else
                transform.localScale = Vector3.one;
        }
    }
}
