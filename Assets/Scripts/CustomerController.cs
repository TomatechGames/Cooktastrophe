using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomerController : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent agent;
    public NavMeshAgent Agent => agent;

    [SerializeField]
    PopupController popupController;
    [SerializeField]
    Animator modelAnimator;
    [SerializeField]
    MeshFilter meshFilter;
    [SerializeField]
    MeshRenderer meshRenderer;

    GrabItemReference grabItem = new();
    public GrabItemEntry GrabItemEntry => grabItem.Entry;
    public bool RecievedFood { get; private set; }

    public Transform PersistentTarget { get; set; }

    private void Update()
    {
        if (PersistentTarget)
            Agent.destination = PersistentTarget.position;
    }

    public void TriggerAnimation(string triggerID)
    {
        modelAnimator.SetTrigger(triggerID);
    }

    Coroutine timerCoroutine;

    public void PickFood()
    {
        grabItem.Id = GameStateManager.Instance.GetRandomFood();
        ApplyItem();
        popupController.SetActive(true);
        timerCoroutine = StartCoroutine(CoroutineHelpers.Timer(GameStateManager.Instance.FoodPatience, popupController.SetPercent, GameStateManager.Instance.GameOver));
    }

    public bool TryDeliverFood(int id)
    {
        if(grabItem.Id == id && !RecievedFood)
        {
            DeliverFood();
            return true;
        }
        return false;
    }

    public void DeliverFood()
    {
        RecievedFood = true;
        popupController.SetActive(false);
        if(timerCoroutine!=null)
            StopCoroutine(timerCoroutine);
    }

    public void ApplyItem()
    {
        if (grabItem.Entry == null)
        {
            meshFilter.mesh = null;
            return;
        }
        meshFilter.mesh = grabItem.Entry.Mesh;
        if (grabItem.Entry.Materials.Count > 0)
        {
            meshRenderer.SetMaterials(grabItem.Entry.Materials);
        }
        meshRenderer.material.mainTexture = grabItem.Entry.DefaultTexture;
    }

    public void ResetState()
    {
        RecievedFood=false;
    }
}
