using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private void Start()
    {
        var socket = GetComponentInChildren<XRSocketInteractor>();
        socket.selectEntered.AddListener(e =>
        {
            if (e.interactableObject is XRGrabInteractable xRGrab && (xRGrab.interactionLayers.value & InteractionLayerMask.GetMask("DirtyPlate"))!=0)
            {
                popupController.SetActive(true);
                washingProcess = StartCoroutine(CoroutineHelpers.Timer(10, popupController.SetPercent, () =>
                {
                    var oldPos = xRGrab.transform.position;
                    var oldRot = xRGrab.transform.rotation;
                    Destroy(xRGrab.gameObject);

                    var spawned = Instantiate(cleanPlatePrefab);
                    spawned.transform.position = oldPos;
                    spawned.transform.rotation = oldRot;
                    socket.StartManualInteraction(spawned as IXRSelectInteractable);

                    popupController.SetActive(false);
                }));
            }
        });
        socket.selectExited.AddListener(e =>
        {
            popupController.SetActive(false);
            if(washingProcess!=null)
                StopCoroutine(washingProcess);
        });
    }

    Coroutine washingProcess;
}
