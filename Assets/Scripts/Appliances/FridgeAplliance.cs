using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FridgeAplliance : MonoBehaviour
{
    XRSocketInteractor fridgeSocket;
    public GameObject grabItemPreFab;
    public GrabItemReference itemReference;
    // Start is called before the first frame update
    void Start()
    {
        fridgeSocket = GetComponent<XRSocketInteractor>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!fridgeSocket.hasSelection)
        {
            var createdItem = Instantiate(grabItemPreFab);
            var grabItemComponent = createdItem.GetComponent<GrabItemComponent>();
            grabItemComponent.SetNewItemID(itemReference.Id);
            fridgeSocket.StartManualInteraction(createdItem.GetComponent<IXRSelectInteractable>());
          
            
        }
        
    }
}
