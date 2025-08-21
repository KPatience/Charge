using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class PlayerBatteryManager : MonoBehaviour
{
    [Header("UI")]
    public Slider BatterySlider;
    
    [Header("Player Data")]
    [SerializeField] private PlayerData _playerData;
    
    private Rigidbody _rb;
    private PlayerController _playerController;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerController = GetComponent<PlayerController>();

        if (_rb == null)
            Debug.LogError("No Rigidbody found on " + gameObject.name);
        
        if (BatterySlider != null)
            BatterySlider.maxValue = _playerData.BatteryLife;
    }


    // need to add some mechanism to drain battery while jump started 
    // need to fix battery drain while running 
    void Update()
    {
        if (_rb == null)
            return;

        float speed = _rb.linearVelocity.magnitude;
        float drain = _playerData.IdleDrain;

        if (speed > 0.1f && speed < _playerData.SprintThreshold)
            drain = _playerData.MoveDrain;
        else if (speed >= _playerData.SprintThreshold)
            drain = _playerData.SprintDrain;

        _playerData.CurrentBatteryLife -= drain * Time.deltaTime;
        _playerData.CurrentBatteryLife = Mathf.Clamp(_playerData.CurrentBatteryLife, 0, _playerData.BatteryLife);

        if (BatterySlider != null)
            BatterySlider.value = _playerData.CurrentBatteryLife;
        
        if (_playerData.CurrentBatteryLife <= 0)
            Die();
    }

    void Die()
    {
        _playerController.Die();    
    }

}

