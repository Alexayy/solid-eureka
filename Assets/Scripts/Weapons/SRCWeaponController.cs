using UnityEngine;

public class SRCWeaponController : MonoBehaviour
{
    private SRCCharacterController _characterController;
    
    [Header("Settings")] 
    public Models.WeaponModel settings;

    private bool _isInitialized;
    
    private Vector3 _newWeaponRotation;
    private Vector3 _newWeaponRotationVelocity;

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
        
        _newWeaponRotation.y += settings.SwayAmount * (settings.SwayXInverted ? -_characterController.inputView.x : _characterController.inputView.x) * Time.deltaTime;
        _newWeaponRotation.x += settings.SwayAmount * (settings.SwayYInverted ? _characterController.inputView.y : -_characterController.inputView.y) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(_newWeaponRotation);
        
        // _newWeaponRotation.x = Mathf.Clamp(_newWeaponRotation.x, viewClampYMin, viewClampYMax);
    }
}
