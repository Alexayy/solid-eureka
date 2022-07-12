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

    private bool _isGroundedTrigger;

    public float fallingDelay;

    [Header("Weapon Sway Breathing")] 
    public Transform weaponSwayObj;

    public float swayAmountA = 1;
    public float swayAmountB = 2;
    public float swayScale = 600;
    public float swayLerpSpeed = 14;

    public float swayTime;
    public Vector3 swayPosition;

    [HideInInspector] 
    public bool isAimingIn;

    [Header("Sights")] 
    public Transform sightTarget;
    public float sightOffset;
    public float aimingInTime;
    private Vector3 weaponSwayPosition;
    private Vector3 weaponSwayPositionVelocity;
    
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
        CalculateWaponSway();

        CalculateAimingIn();
    }

    private void CalculateAimingIn()
    {
        var targetPosition = transform.position;

        if (isAimingIn)
        {
            targetPosition = _characterController.cameraHolder.transform.position + 
                             (weaponSwayObj.transform.position - sightTarget.position) + 
                             (_characterController.cameraHolder.transform.forward * sightOffset);
        }
        
        weaponSwayPosition = weaponSwayObj.transform.position;
        weaponSwayPosition = Vector3.SmoothDamp(weaponSwayPosition, targetPosition, ref weaponSwayPositionVelocity, aimingInTime);

        weaponSwayObj.transform.position = weaponSwayPosition;
    }

    public void TriggerJump()
    {
        _isGroundedTrigger = false;
        weaponAnimator.SetTrigger("Jump");
    }

    private void CalculateWeaponRotation()
    {
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
        if (_isGroundedTrigger)
        {
            fallingDelay = 0;
        }
        else
        {
            fallingDelay += Time.deltaTime;
        }

        if (_characterController.isGrounded && !_isGroundedTrigger && fallingDelay > 0.1f)
        {
            weaponAnimator.SetTrigger("Land");
            _isGroundedTrigger = true;
        }
        else if (!_characterController.isGrounded && _isGroundedTrigger)
        {
            weaponAnimator.SetTrigger("Falling");
            _isGroundedTrigger = false;
        }
        
        weaponAnimator.SetBool("IsSprinting", _characterController.isSprinting);
        weaponAnimator.SetFloat("weaponAnimationSpeed", _characterController.weaponAnimationSpeed);
    }

    private void CalculateWaponSway()
    {
        var targetPosition = LissajousCurve(swayTime, swayAmountA, swayAmountB) / swayScale;

        swayPosition = Vector3.Lerp(swayPosition, targetPosition, Time.smoothDeltaTime * swayLerpSpeed);
        swayTime += Time.deltaTime;

        if (swayTime > 6.3f)
        {
            swayTime = 0;
        }

        // weaponSwayObj.localPosition = swayPosition;
    }
    
    private Vector3 LissajousCurve(float time, float a, float b)
    {
        return new Vector3(Mathf.Sin(time), a * Mathf.Sin(b * time + Mathf.PI));
    }
}
