using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class HobAppliance : MonoBehaviour
{
    XRSocketInteractor itemSocket;
    public float processTime;
    public float totalProcessTime;
    public ProcessType processType;
    public UnityEvent<float> onProcessTimeUpdate;
    public UnityEvent<bool> onProcessActivityUpdate;
    // Start is called before the first frame update
    void Start()
    {
        itemSocket = GetComponentInChildren<XRSocketInteractor>();
        itemSocket.selectEntered.AddListener(e => StartProcess(e.interactableObject));
        itemSocket.selectExited.AddListener(e => StopProcess());
    }

    public void StartProcess(IXRSelectInteractable interactable)
    {
        if((interactable as MonoBehaviour).TryGetComponent<GrabItemComponent>(out var grabItem))
        {
            var recipe = GrabItemDatabaseHolder.Database.GetProcessEntry(grabItem.GrabItem.Id, processType);
            if (recipe == null)
            {
                onProcessActivityUpdate.Invoke(false);
                return;
            }
            processTime = totalProcessTime;
            onProcessTimeUpdate.Invoke(0);
            onProcessActivityUpdate.Invoke(true);
        }
        else
            onProcessActivityUpdate.Invoke(false);
    }
    
    public void StopProcess()
    {
        processTime = -1;
        onProcessActivityUpdate.Invoke(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (processTime > 0)
        {
            onProcessTimeUpdate.Invoke((totalProcessTime-processTime)/totalProcessTime);
            processTime = processTime - Time.deltaTime;
            if (processTime < 0)
            {
                if ((itemSocket.firstInteractableSelected as MonoBehaviour).TryGetComponent<GrabItemComponent>(out var grabItem))
                {
                    var recipe = GrabItemDatabaseHolder.Database.GetProcessEntry(grabItem.GrabItem.Id, processType);
                    if (recipe == null)
                    {
                        onProcessActivityUpdate.Invoke(false);
                        return;
                    }
                    grabItem.SetNewItemID(recipe.Result);
                    StartProcess(itemSocket.firstInteractableSelected);
                }
            }
        }
    }
}
