using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [SerializeField] float lookUpDistance;
    [SerializeField] float lookDownDistance;

    private CameraFollow cameraFollow;
    private CameraInputActions cameraInputActions;

    private bool isLookHeld = false;

    void Awake()
    {
        cameraFollow = GetComponent<CameraFollow>();

        cameraInputActions = new CameraInputActions();
        cameraInputActions.Camera.Enable();
        cameraInputActions.Camera.LookUpDown.performed += LookUpDown_performed;
        cameraInputActions.Camera.LookUpDown.canceled += LookUpDown_canceled;
    }

    private void LookUpDown_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        float input = cameraInputActions.Camera.LookUpDown.ReadValue<float>();

        if(input == 1)
            cameraFollow.cameraControlsOffset += lookUpDistance;
        else
            cameraFollow.cameraControlsOffset += -lookDownDistance;
    }

    private void LookUpDown_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        cameraFollow.cameraControlsOffset = 0;
    }
}
