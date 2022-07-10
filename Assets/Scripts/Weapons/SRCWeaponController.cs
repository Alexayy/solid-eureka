using UnityEngine;

public class SRCWeaponController : MonoBehaviour
{
    private SRCCharacterController _characterController;

    [Header("References")] 
    public Animator weaponAnimator;
    
    [Header("Settings")] 
    public Models.WeaponModel settings;

    private bool _isInitialized;
    
    private Vector3 _newWeaponRotation;
    private Vector3 _newWeaponRotationVelocity;
    
    private Vector3 _targetWeaponRotation;
    private Vector3 _targetWeaponRotationVelocity;
    
    private Vector3 _newWeaponMovementRotation;
    private Vector3 _newWeaponMovementRotationVelocity;
    
    private Vector3 _targetWeaponMovementRotation;
    private Vector3 _targetWeaponMovementRotationVelocity;
    
    private void Start()
    {
        _newWeaponRotation = transform.localRotation.eulerAngles;
    }

    public void Initialize(SRCCharacterController characterController)
    {
        _characterController = characterController;
        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized)
        {
            return;
        }

        CalculateWeaponRotation();
        SetWeaponAnimations();
    }

    private void CalculateWeaponRotation()
    {
        weaponAnimator.speed = _characterController.weaponAnimationSpeed;
        
        _targetWeaponRotation.y += settings.SwayAmount * (settings.SwayXInverted ? -_characterController.inputView.x : _characterController.inputView.x) * Time.deltaTime;
        _targetWeaponRotation.x += settings.SwayAmount * (settings.SwayYInverted ? _characterController.inputView.y : -_characterController.inputView.y) * Time.deltaTime;
        
        _targetWeaponRotation.x = Mathf.Clamp(_targetWeaponRotation.x, -settings.SwayClampX, settings.SwayClampY);
        _targetWeaponRotation.y = Mathf.Clamp(_targetWeaponRotation.y, settings.SwayClampX, settings.SwayClampY);
        _targetWeaponRotation.z = _targetWeaponRotation.y;
        
        _targetWeaponRotation = Vector3.SmoothDamp(_targetWeaponRotation, Vector3.zero, ref _targetWeaponRotationVelocity, settings.SwayResetSmoothing);
        _newWeaponRotation = Vector3.SmoothDamp(_newWeaponRotation, _targetWeaponRotation, ref _newWeaponRotationVelocity, settings.SwaySmoothing);

        _targetWeaponMovementRotation.z = settings.MovementSwayX * (settings.MovementSwayXInverted ? -_characterController.inputMovement.x : _characterController.inputMovement.x);
        _targetWeaponMovementRotation.x = settings.MovementSwayY * (settings.MovementSwayYInverted ? -_characterController.inputMovement.y : _characterController.inputMovement.y);
        
        _targetWeaponMovementRotation = Vector3.SmoothDamp(_targetWeaponMovementRotation, Vector3.zero, ref _targetWeaponMovementRotationVelocity, settings.MovementSwaySmoothing);
        _newWeaponMovementRotation = Vector3.SmoothDamp(_newWeaponMovementRotation, _targetWeaponMovementRotation, ref _newWeaponMovementRotationVelocity, settings.MovementSwaySmoothing);
        
        transform.localRotation = Quaternion.Euler(_newWeaponRotation + _newWeaponMovementRotation);
    }

    private void SetWeaponAnimations()
    {
        weaponAnimator.SetBool("IsSprinting", _characterController.isSprinting);
    }
}
