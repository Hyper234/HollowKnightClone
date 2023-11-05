using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [SerializeField] float lookUpDistanceModifier;
    [SerializeField] float lookDownDistanceModifier;

    private Camera camera;
    private CameraFollow cameraFollow;
    private CameraInputActions cameraInputActions;

    private float lookUpDistance;
    private float lookDownDistance;

    void Awake()
    {
        camera = GetComponent<Camera>();
        cameraFollow = GetComponent<CameraFollow>();

        lookUpDistance = camera.orthographicSize * lookUpDistanceModifier;
        lookDownDistance = camera.orthographicSize * lookDownDistanceModifier;

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
