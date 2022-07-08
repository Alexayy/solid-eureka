using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRCCharacterController : MonoBehaviour
{
    private CharacterController _characterController;

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

    [Header("Gravity")] 
    public float gravityAmount;
    public float gravityMin;
    private float playerGravity;

    public Vector3 jumpingForce;
    private Vector3 _jumpingForceVelocity;

    [Header("Stance")] 
    public Models.PlayerStance playerStance;
    public float playerStanceSmoothing;

    public Models.CharacterStance playerStandStance;
    public Models.CharacterStance playerCrouchStance;
    public Models.CharacterStance playerProneStance;
    
    private float _cameraHeight;
    private float _cameraHeightVelocity;

    private Vector3 stanceCapsuleCenter;
    private Vector3 stanceCapsuleCenterVelocity;

    private float stanceCapsuleHeight;
    private float stanceCapsuleHeightVelocity;

    private void Awake()
    {
        _defaultInput = new DefaultInput();

        _defaultInput.Character.Movement.performed += e => inputMovement = e.ReadValue<Vector2>();
        _defaultInput.Character.View.performed += e => inputView = e.ReadValue<Vector2>();
        _defaultInput.Character.Jump.performed += e => Jump();

        _defaultInput.Enable();

        _newCameraRotation = cameraHolder.localRotation.eulerAngles;
        _newPlayerRotation = transform.localRotation.eulerAngles;

        _characterController = GetComponent<CharacterController>();

        _cameraHeight = cameraHolder.localRotation.y;
    }

    private void Update()
    {
        CalculateView();
        CalculateMove();
        CalculateJump();
        CalculateStance();
    }

    private void CalculateView()
    {
        _newPlayerRotation.y += playerSettings.ViewXSensitivity *
                                (playerSettings.ViewXInverted ? -inputView.x : inputView.x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(_newPlayerRotation);

        _newCameraRotation.x += playerSettings.ViewYSensitivity *
                                (playerSettings.ViewYInverted ? inputView.y : -inputView.y) * Time.deltaTime;
        _newCameraRotation.x = Mathf.Clamp(_newCameraRotation.x, viewClampYMin, viewClampYMax);

        cameraHolder.localRotation = Quaternion.Euler(_newCameraRotation);
    }

    private void CalculateMove()
    {
        var verticalSpeed = playerSettings.ForwardSpeed * inputMovement.y * Time.deltaTime;
        var horizontalSpeed = playerSettings.StrafeSpeed * inputMovement.x * Time.deltaTime;

        var newMoveSpeed = new Vector3(horizontalSpeed, 0, verticalSpeed);
        newMoveSpeed = transform.TransformDirection(newMoveSpeed);

        if (playerGravity > gravityMin)
        {
            playerGravity -= gravityAmount * Time.deltaTime;
        }

        if (playerGravity < -0.1f && _characterController.isGrounded)
        {
            playerGravity = -0.1f;
        }

        newMoveSpeed.y += playerGravity;
        newMoveSpeed += jumpingForce * Time.deltaTime;

        _characterController.Move(newMoveSpeed);
    }

    private void CalculateJump()
    {
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref _jumpingForceVelocity,
            playerSettings.JumpingFallOff);
    }

    private void CalculateStance()
    {
        var currentStance = playerStandStance;

        if (playerStance == Models.PlayerStance.Crouch)
        {
            currentStance = playerCrouchStance;
        }
        else if (playerStance == Models.PlayerStance.Prone)
        {
            currentStance = playerProneStance;
        }
        
        _cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, currentStance.CameraHeight, ref _cameraHeightVelocity, playerStanceSmoothing);
        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, _cameraHeight, cameraHolder.localPosition.z);

        _characterController.height = Mathf.SmoothDamp(_characterController.height, currentStance.StanceCollider.height, ref stanceCapsuleHeightVelocity, playerStanceSmoothing);
        _characterController.center = Vector3.SmoothDamp(_characterController.center,
            currentStance.StanceCollider.center, ref stanceCapsuleCenterVelocity, playerStanceSmoothing);
    }
    
    private void Jump()
    {
        if (!_characterController.isGrounded)
            return;

        jumpingForce = Vector3.up * playerSettings.JumpingHeight;
        playerGravity = 0;
    }
}