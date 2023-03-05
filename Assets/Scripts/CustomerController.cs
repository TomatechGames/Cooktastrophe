using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomerController : MonoBehaviour
{
    public NavMeshAgent agent;
    public XRSocketInteractor tableSocket;
    public Transform chairTransform;
    public Transform outsideTarget;
    public PopupController popupController;
    public Animator modelAnimator;
    public GameObject unknownOrderIcon;
    public GameObject foodIconA;
    public GameObject foodIconB;
    public GameObject foodIconC;
    public InputActionReference takeOrderButton;

    public bool DishDelivered { get; set; }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => takeOrderButton.action.IsPressed());
        agent.destination = chairTransform.position;
        modelAnimator.SetTrigger("Walk");
        yield return new WaitUntil(() =>(transform.position - chairTransform.position).magnitude<0.5f);
        agent.enabled = false;
        transform.position = chairTransform.position;
        transform.rotation = Quaternion.Euler(chairTransform.rotation.eulerAngles+new Vector3(0,180,0));
        modelAnimator.SetTrigger("Sit");
        popupController.SetActive(true);
        takeOrderButton.action.Enable();
        unknownOrderIcon.SetActive(true);
        var foodRoutine = StartCoroutine(FoodProcess());
        yield return new WaitUntil(() => takeOrderButton.action.IsPressed());
        unknownOrderIcon.SetActive(false);
        switch (Random.Range(0, 2))
        {
            case 0:
                foodIconA.SetActive(true);
                break;
            case 1:
                foodIconB.SetActive(true);
                break;
            case 2:
                foodIconC.SetActive(true);
                break;

        }
        yield return new WaitUntil(() => DishDelivered);
        StopCoroutine(foodRoutine);
        modelAnimator.SetTrigger("Eat");
        foodIconA.SetActive(false);
        foodIconB.SetActive(false);
        foodIconC.SetActive(false);
        float elapsed = 0;
        while (elapsed<10)
        {
            elapsed += Time.deltaTime;
            popupController.SetPercent(elapsed/10);
            yield return null;
        }
        popupController.SetActive(false);
        Destroy(((tableSocket.firstInteractableSelected as MonoBehaviour).GetComponentInChildren<XRSocketInteractor>().firstInteractableSelected as MonoBehaviour).gameObject);
        Destroy((tableSocket.firstInteractableSelected as MonoBehaviour).gameObject);
        agent.enabled = true;
        agent.destination = outsideTarget.position;
        modelAnimator.SetTrigger("Walk");
        StartCoroutine(Start());
    }

    IEnumerator FoodProcess()
    {
        float elapsed = 0;
        while (elapsed < 90)
        {
            elapsed += Time.deltaTime;
            popupController.SetPercent(elapsed / 90);
            yield return null;
        }
    }
}
