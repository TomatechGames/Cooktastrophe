using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlateRackAppliance : MonoBehaviour, IApplianceLogic
{
    [SerializeField]
    List<XRSocketInteractor> plateSockets;
    [SerializeField]
    XRGrabInteractable platePrefab;

    ApplianceCore applianceCore;
    public ApplianceCore ApplianceCore => applianceCore ??= GetComponentInParent<ApplianceCore>();

    private void Start()
    {

        GameStateManager.Instance.OnStateChange += s => SetActive(s == GameStateManager.GameState.Dining);
    }

    private void SetActive(bool value)
    {
        if (value )
        {
            foreach (var socket in plateSockets)
            {
                if (socket.isPerformingManualInteraction)
                    socket.EndManualInteraction();
                var spawned = Instantiate(platePrefab);
                spawned.transform.position = socket.transform.position;
                socket.StartManualInteraction(spawned as IXRSelectInteractable);
            }
        }
        else
        {
            foreach (var socket in plateSockets)
            {
                if (socket.hasSelection)
                {
                    Destroy(socket.firstInteractableSelected.AsBehavior().gameObject);
                }
            }
        }
    }
}
