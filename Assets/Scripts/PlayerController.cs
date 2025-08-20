using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    public float runSpeed = 12f;
    public float rotationSpeed = 5f;
    public bool canMove = true;

    private Rigidbody rb;
    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }


    void FixedUpdate()
    {

        

        if (isDead)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); // Let gravity continue
            animator.SetBool("IsMoving", false);
            animator.SetBool("IsRunning", false);
            return;
        }

        float moveInput = -Input.GetAxis("Horizontal");
        float currentSpeed = Input.GetButton("Sprint") ? runSpeed : speed;
        float dirZ = moveInput * currentSpeed;

        animator.SetBool("IsRunning", Input.GetButton("Sprint"));
        animator.SetBool("IsActivating", Input.GetButton("Activate"));

        Vector3 move = new Vector3(0, rb.linearVelocity.y, -dirZ);
        rb.linearVelocity = move;

        Vector3 flatMove = new Vector3(0, 0, dirZ);
        if (flatMove != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatMove);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        animator.SetBool("IsMoving", moveInput != 0);
        
    }

    // Call this method to trigger death
    public void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        // Disable the script after triggering the animation
        this.enabled = false;
    }

}