using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Management;

public class VRPlayerInput : MonoBehaviour
{
    [SerializeField]
    InputActionReference movementInput;
    [SerializeField]
    float moveSpeed = 1f;
    [SerializeField]
    Vector2 latestMoveInput;

    [Space]
    [SerializeField]
    Rigidbody rigRef;
    [SerializeField]
    Transform leftHandRef;


    private void Start()
    {
        if (!XRGeneralSettings.Instance.Manager.activeLoader)
        {
            enabled = false;
        }
    }

    private void OnEnable()
    {

        movementInput.action.performed += OnMoveInput;
        movementInput.action.canceled += OnMoveInput;

        movementInput.action.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;

        movementInput.action.performed -= OnMoveInput;
        movementInput.action.canceled -= OnMoveInput;

        movementInput.action.Disable();
    }

    private void OnMoveInput(InputAction.CallbackContext obj)
    {
        latestMoveInput = obj.ReadValue<Vector2>();
    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        rigRef.velocity = (
            (
                (FlattenDir(leftHandRef.forward) * latestMoveInput.y) +
                (FlattenDir(leftHandRef.right) * latestMoveInput.x)
            ) *
            (moveSpeed));
    }


    public Vector3 FlattenDir(Vector3 initial)
    {
        initial.y = 0;
        return initial;
    }
}
