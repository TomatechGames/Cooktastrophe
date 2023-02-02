using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Management;

public class DeveloperPlayerInput : BasePlatformInput
{
    [SerializeField]
    InputActionReference movementInput;
    [SerializeField]
    InputActionReference mouseDelta;
    [SerializeField]
    Vector2 pitchAndYaw;
    [SerializeField]
    Vector2 latestMoveInput;
    [SerializeField]
    float sensitivity = 0.25f;

    [Space]
    [SerializeField]
    Transform leftHandRef;
    [SerializeField]
    Transform rightHandRef;
    [SerializeField]
    Transform leftHandTarget;
    [SerializeField]
    Transform rightHandTarget;

    public override Vector3 MoveDir => 
                (FlattenDir(transform.forward) * latestMoveInput.y) +
                (FlattenDir(transform.right) * latestMoveInput.x);

    private void Start()
    {
        if (!XRGeneralSettings.Instance.Manager.activeLoader)
            GetComponentInParent<PlayerMovementManager>().PlatformInput = this;
        else
            enabled = false;
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;

        mouseDelta.action.performed += OnMouseDelta;
        movementInput.action.performed += OnMoveInput;
        movementInput.action.canceled += OnMoveInput;

        mouseDelta.action.Enable();
        movementInput.action.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;

        mouseDelta.action.performed -= OnMouseDelta;
        movementInput.action.performed -= OnMoveInput;
        movementInput.action.canceled -= OnMoveInput;

        mouseDelta.action.Disable();
        movementInput.action.Disable();
    }

    private void OnMoveInput(InputAction.CallbackContext obj)
    {
        latestMoveInput = obj.ReadValue<Vector2>();
    }

    private void OnMouseDelta(InputAction.CallbackContext obj)
    {
        var rawValue = obj.ReadValue<Vector2>() * sensitivity;
        pitchAndYaw += new Vector2(-rawValue.y,rawValue.x);
    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pitchAndYaw.x = Mathf.Clamp(pitchAndYaw.x, -89, 89);
        pitchAndYaw.y = ((pitchAndYaw.y+540)%360)-180;

        transform.rotation = Quaternion.Euler(pitchAndYaw);

        if (leftHandRef && leftHandTarget)
            leftHandRef.SetPositionAndRotation(leftHandTarget.position, leftHandTarget.rotation);
        if (rightHandRef && rightHandTarget)
            rightHandRef.SetPositionAndRotation(rightHandTarget.position, rightHandTarget.rotation);
    }

    public Vector3 FlattenDir(Vector3 initial)
    {
        initial.y = 0;
        return initial.normalized;
    }


    static float UnsignedModulo(float value, float wrapPoint)
    {
        float wrapped = value % wrapPoint;
        return wrapped < 0 ? wrapped : wrapPoint + wrapped;
    }
}
