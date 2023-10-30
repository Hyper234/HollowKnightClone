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
    }

    private void LookUpDown_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        float input = cameraInputActions.Camera.LookUpDown.ReadValue<float>();

        if (!isLookHeld)
        {
            if(input == 1)
                cameraFollow.cameraControlsOffset += lookUpDistance;
            else
                cameraFollow.cameraControlsOffset += -lookDownDistance;
        }
        else
        {
            cameraFollow.cameraControlsOffset = 0;
        }

        isLookHeld = !isLookHeld;
    }
}
