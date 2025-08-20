using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class BatteryManager : MonoBehaviour
{
    public float batteryLife = 100f;
    public float idleDrain = 1f;
    public float moveDrain = 3f;
    public float sprintDrain = 7f;
    public float sprintThreshold = 5f;
    private Animator animator;
    private Rigidbody rb;
    public Slider batterySlider; // Reference to UI Slider

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();

        if (rb == null)
            Debug.LogError("No Rigidbody found on " + gameObject.name);

        if (batterySlider != null)
            batterySlider.maxValue = 100f;
    }

    void Update()
    {
        if (rb == null)
            return;

        float speed = rb.linearVelocity.magnitude;
        float drain = idleDrain;

        if (speed > 0.1f && speed < sprintThreshold)
            drain = moveDrain;
        else if (speed >= sprintThreshold)
            drain = sprintDrain;

        batteryLife -= drain * Time.deltaTime;
        batteryLife = Mathf.Clamp(batteryLife, 0, 100f);

        if (batterySlider != null)
            batterySlider.value = batteryLife;

        if (batteryLife <= 0)
            Die();
    }

    void Die()
    {
       
        animator.SetTrigger("Die");
        // Game over logic
    }

}

