using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class BatteryManager : MonoBehaviour
{
    public float BatteryLife = 100f;
    public float IdleDrain = 1f;
    public float MoveDrain = 3f;
    public float SprintDrain = 7f;
    public float SprintThreshold = 5f;
    private Animator _animator;
    private Rigidbody _rb;
    public Slider BatterySlider; // Reference to UI Slider

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _animator = GetComponent<Animator>();

        if (_rb == null)
            Debug.LogError("No Rigidbody found on " + gameObject.name);

        if (BatterySlider != null)
            BatterySlider.maxValue = 100f;
    }

    void Update()
    {
        if (_rb == null)
            return;

        float speed = _rb.linearVelocity.magnitude;
        float drain = IdleDrain;

        if (speed > 0.1f && speed < SprintThreshold)
            drain = MoveDrain;
        else if (speed >= SprintThreshold)
            drain = SprintDrain;

        BatteryLife -= drain * Time.deltaTime;
        BatteryLife = Mathf.Clamp(BatteryLife, 0, 100f);

        if (BatterySlider != null)
            BatterySlider.value = BatteryLife;

        if (BatteryLife <= 0)
            Die();
    }

    void Die()
    {
       
        _animator.SetTrigger("Die");
        // Game over logic
    }

}

