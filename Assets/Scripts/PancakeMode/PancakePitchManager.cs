using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;
using UnityEngine.XR.Management;

public class PancakePitchManager : MonoBehaviour
{
    [SerializeField]
    InputActionReference mousePitchDelta;
    [SerializeField]
    float turnSpeed;
    [SerializeField]
    float pitch;

    private void Start()
    {
        if (XRGeneralSettings.Instance.Manager.activeLoader)
            enabled = false;
        pitch = 0;
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;

        mousePitchDelta.action.performed += OnMouseDelta;

        mousePitchDelta.action.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;

        mousePitchDelta.action.performed -= OnMouseDelta;

        mousePitchDelta.action.Disable();
    }

    private void OnMouseDelta(InputAction.CallbackContext obj)
    {
        var input = obj.ReadValue<Vector2>();
        pitch -= input.magnitude * (Mathf.Sign(input.y) * turnSpeed * Time.deltaTime);
    }

    private void Update() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        pitch = Mathf.Clamp(pitch, -89, 89);

        transform.localRotation = Quaternion.Euler(pitch, 0, 0);
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