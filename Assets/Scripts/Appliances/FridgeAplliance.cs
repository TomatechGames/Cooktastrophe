using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FridgeAplliance : MonoBehaviour
{
    XRSocketInteractor fridgeSocket;
    public GameObject grabItemPrefab;
    public GrabItemReference itemReference;

    // Start is called before the first frame update
    void Start()
    {
        fridgeSocket = GetComponentInChildren<XRSocketInteractor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!fridgeSocket.hasSelection)
        {
            var createdItem = Instantiate(grabItemPrefab);
            createdItem.transform.position = transform.position;
            fridgeSocket.StartManualInteraction(createdItem.GetComponent<IXRSelectInteractable>());
            if (itemReference.Id == 0)
                return;
            var grabItemComponent = createdItem.GetComponent<GrabItemComponent>();
            grabItemComponent.SetNewItemID(itemReference.Id);
        }
    }
}
