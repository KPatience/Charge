using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 6f;
    public float RunSpeed = 12f;
    public float RotationSpeed = 5f;
    public bool CanMove = true;

    private Rigidbody _rb;
    private Animator _animator;
    private bool _isDead = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }


    void FixedUpdate()
    {

        

        if (_isDead)
        {
            _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0); // Let gravity continue
            _animator.SetBool("IsMoving", false);
            _animator.SetBool("IsRunning", false);
            return;
        }

        float moveInput = -Input.GetAxis("Horizontal");
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? RunSpeed : Speed;
        float dirZ = moveInput * currentSpeed;


        _animator.SetBool("IsRunning", Input.GetKeyDown(KeyCode.LeftShift));
        _animator.SetBool("IsActivating", Input.GetKey(KeyCode.E));

        Vector3 move = new Vector3(0, _rb.linearVelocity.y, -dirZ);
        _rb.linearVelocity = move;

        Vector3 flatMove = new Vector3(0, 0, dirZ);
        if (flatMove != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatMove);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }

        _animator.SetBool("IsMoving", moveInput != 0);
        
    }

    // Call this method to trigger death
    public void Die()
    {
        _isDead = true;
        _animator.SetTrigger("Die");

        // Disable the script after triggering the animation
        this.enabled = false;
    }

}