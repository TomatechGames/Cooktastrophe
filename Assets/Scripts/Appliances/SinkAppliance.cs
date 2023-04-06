using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SinkAppliance : MonoBehaviour, IApplianceLogic
{
    [SerializeField]
    XRGrabInteractable cleanPlatePrefab;
    [SerializeField]
    PopupController popupController;

    ApplianceCore applianceCore;
    public ApplianceCore ApplianceCore => applianceCore ??= GetComponentInParent<ApplianceCore>();
}
