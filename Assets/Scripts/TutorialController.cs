using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    static TutorialController instance;
    public static TutorialController Instance => instance;

    [SerializeField]
    Transform hintArrow;
    [SerializeField]
    Transform hintMarker;
    [SerializeField]
    Transform hintLabel;
    [SerializeField]
    TextMeshPro tutorialText;
    int tutorialStage = 0;

    [SerializeField]
    Transform[] pois;

    IEnumerator Start()
    {
        instance = this;
        if (MainMenuUIManager.StartAtDay != 0)
        {
            hintArrow.localScale = Vector3.zero;
            hintMarker.localScale = Vector3.zero;
            yield break;
        }

        //0
        tutorialText.text = 
            "Welcome to Cooktastrophe! " +
            "You can press the Grip button (middle finger) to pick up objetcs. " +
            "Try it out with this counter.";
        yield return WaitForNextStage();
        tutorialStage--;
        yield return WaitForNextStage();

        //1
        tutorialText.text =
            "Now, press the Grip button (middle finger) again to place the object down. " +
            "You can only place objects when theres something to place them on.";
        hintArrow.localScale = Vector3.zero;
        hintMarker.localScale = Vector3.zero;
        yield return WaitForNextStage();

        //2
        tutorialText.text =
            "Feel free to rearange the appliances to your liking, " +
            "and press this button to start the work day. " +
            "Use the Trigger button (index finger) while hovering the Start Day button";
        hintArrow.localScale = Vector3.one;
        hintMarker.localScale = Vector3.one;
        hintMarker.position = pois[0].position;
        yield return WaitForNextStage();

        //3
        tutorialText.text = "";
        hintArrow.localScale = Vector3.zero;
        hintMarker.localScale = Vector3.zero;
        yield return WaitForNextStage();

        //4
        tutorialText.text =
            "A customer has arrived and sat down, " +
            "you now have a limited amount of time to take their order. " +
            "Use the Trigger button (index finger) on the Table.";
        hintArrow.localScale = Vector3.one;
        hintMarker.position = pois[1].position;
        yield return WaitForNextStage();

        //5
        tutorialText.text =
            "They have ordered some meat. " +
            "Go to the fridge and take out some raw meat using the Grip (middle finger) button.";
        hintMarker.position = pois[2].position;
        hintMarker.localScale = Vector3.one;
        yield return WaitForNextStage();

        //6
        tutorialText.text =
            "Place the meat on the Hob.";
        hintMarker.position = pois[3].position;
        yield return WaitForNextStage();

        //7
        hintMarker.localScale = Vector3.zero;
        hintArrow.localScale = Vector3.zero;
        tutorialText.text =
            "Wait for the meat to match what the customer ordered. " +
            "If the meat gets overdone, put it in the bin and cook a new piece of meat";
        yield return WaitForNextStage();

        //8
        hintMarker.localScale = Vector3.one;
        hintArrow.localScale = Vector3.one;
        tutorialText.text =
            "When the meat is ready, place it on a plate, " +
            "and place the plate on the table to serve it";
        hintMarker.position = pois[4].position;
        yield return WaitForNextStage();

        //9
        hintArrow.localScale = Vector3.zero;
        hintMarker.localScale = Vector3.zero;
        tutorialText.text = "";
        yield return WaitForNextStage();

        //10
        hintMarker.position = pois[5].position;
        hintMarker.localScale = Vector3.one;
        hintArrow.localScale = Vector3.one;
        tutorialText.text = "Completing a day will grant you a new Appliance!";
        yield return WaitForNextStage();

        hintArrow.localScale = Vector3.zero;
        hintMarker.localScale = Vector3.zero;
        tutorialText.text = "";
    }

    private void Update()
    {
        hintArrow.LookAt(hintMarker);
        hintLabel.Rotate(new(0, 90 * Time.deltaTime, 0));
    }

    IEnumerator WaitForNextStage(int steps = 1)
    {
        yield return WaitForStage(tutorialStage + steps);
    }

    public IEnumerator WaitForStage(int toWaitFor)
    {
        yield return new WaitUntil(() => tutorialStage >= toWaitFor);
    }

    public void AdvanceStage(int fromStage)
    {
        if (tutorialStage == fromStage)
            tutorialStage++;
    }
}
