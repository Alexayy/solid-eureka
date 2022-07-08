using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private DefaultInput _defaultInput;
    public Vector2 inputMovement;
    public Vector2 inputView;

    private Vector2 _newCameraRotation;
    private Vector2 _newPlayerRotation;

    [Header("References")] 
    public Transform cameraHolder;

    [Header("Settings")] 
    public Models.PlayerSettingsModel playerSettings;
    public float viewClampYMin = -70;
    public float viewClampYMax = 80;
    
    private void Awake()
    {
        _defaultInput = new DefaultInput();

        _defaultInput.Character.Movement.performed += e => inputMovement = e.ReadValue<Vector2>();
        _defaultInput.Character.View.performed += e => inputView = e.ReadValue<Vector2>();
        
        _defaultInput.Enable();

        _newCameraRotation = cameraHolder.localRotation.eulerAngles;
        _newPlayerRotation = transform.localRotation.eulerAngles;
    }

    private void Update()
    {
        CalculateView();
        CalculateMove();
    }

    private void CalculateView()
    {
        _newPlayerRotation.y += playerSettings.ViewXSensitivity * inputView.x * Time.deltaTime;
        transform.rotation = cameraHolder.localRotation = Quaternion.Euler(_newPlayerRotation);
        
        _newCameraRotation.x += playerSettings.ViewYSensitivity * inputView.y * Time.deltaTime;
        _newCameraRotation.x = Mathf.Clamp(_newCameraRotation.x, viewClampYMin, viewClampYMax);
        
        cameraHolder.localRotation = Quaternion.Euler(_newCameraRotation);
    }

    private void CalculateMove()
    {
        
    }
}
