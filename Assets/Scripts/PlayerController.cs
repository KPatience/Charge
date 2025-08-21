using Mono.Cecil.Cil;
using System;
using UnityEngine;

// this controller is using the old Input System.
// need to update it to use the new Input System if necessary.
// all animation is controlling form here 
// Modify PlayerData.cs to change player stats

public class PlayerController : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private PlayerData _playerData;

    private Rigidbody _rb;
    private Animator _animator;
    private bool _isDead = false;

    private float _moveInput;
    private float _currentSpeed;
    private bool _isRunning = false;
    private bool _isGrounded = false;
    private bool _jumpPressed = false;
    private float _jumpStartY;

    [Header("JumpSettings")]
    [SerializeField] private float _jumpGroundCheckDistance = 5f;


    public static event Action OnPlayerDead = delegate { };

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
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


        TakeInputs();

        // Ground check using raycast
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _jumpGroundCheckDistance);

        // Handle jump
        if (_jumpPressed && _isGrounded)
        {
            _rb.AddForce(Vector3.up * _playerData.JumpForce, ForceMode.Impulse);
            _jumpStartY = transform.position.y;
            //_animator.SetTrigger("Jump");
        }

        // Limit jump height and downward velocity
        if (transform.position.y > _jumpStartY + _playerData.MaxJumpHeight)
        {
            if (_rb.linearVelocity.y > 0)
            {
                _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
            }
        }

        // Clamp downward velocity
        if (_rb.linearVelocity.y < _playerData.MaxDownwardVelocity)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _playerData.MaxDownwardVelocity, _rb.linearVelocity.z);
        }

        float dirZ = _moveInput * _currentSpeed;

        _animator.SetBool("IsRunning", _isRunning);

        Vector3 move = new Vector3(0, _rb.linearVelocity.y, dirZ);
        _rb.linearVelocity = move;

        Vector3 flatMove = new Vector3(0, 0, dirZ);
        if (flatMove != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatMove);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _playerData.RotationSpeed * Time.deltaTime);
        }
        _animator.SetBool("IsMoving", _moveInput != 0);
    }

    void TakeInputs()
    {
        _moveInput = Input.GetAxis("Horizontal");
        _isRunning = Input.GetKey(KeyCode.LeftShift);
        _currentSpeed = _isRunning ? _playerData.RunSpeed : _playerData.Speed;
        _jumpPressed = Input.GetKey(KeyCode.Space);
        Debug.Log($"Move Input: {_moveInput}, Is Running: {_isRunning}, Current Speed: {_currentSpeed}, Jump Pressed: {_jumpPressed}");
    }


    // Call this method to trigger death
    public void Die()
    {
        _isDead = true;
        _animator.SetTrigger("Die");
        this.enabled = false;
        OnPlayerDead?.Invoke();
    }

    private void OnDrawGizmos()
    {
        // Draw ground check ray
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * _jumpGroundCheckDistance);
    }
}