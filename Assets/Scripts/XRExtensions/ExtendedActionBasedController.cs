using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//This extension makes Selections operate on a toggle. Pressing the grip button with no object selected will try to select the object, otherwise it will deselect the current object
public class ExtendedActionBasedController : ActionBasedController
{
    [SerializeField]
    XRBaseInteractor interactor;
    [SerializeField]
    bool selectBuffer;

    protected override void UpdateInput(XRControllerState controllerState)
    {
        base.UpdateInput(controllerState);
        if (controllerState == null || !interactor)
            return;

        controllerState.selectInteractionState.ResetFrameDependent();
        var selectValueAction = selectActionValue.action;
        if (selectValueAction == null || selectValueAction.bindings.Count <= 0)
            selectValueAction = selectAction.action;

        bool selectPressed = IsPressed(selectAction.action);
        bool shouldBeSelected = (selectPressed && !selectBuffer) == !interactor.hasSelection;

        controllerState.selectInteractionState.SetFrameState(shouldBeSelected, ReadValue(selectValueAction));

        selectBuffer = selectPressed;
    }
}
