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
    private bool _jumpHeld = false;
    private bool _wasJumpHeld = false;
    private float _jumpStartY;
    private float _jumpHoldTimer = 0f;
    private bool _isJumping = false;
    private bool _wasGrounded = false;
    private bool _isAtPeak = false;
    private float _currentGravityScale = 1f;
    private float _previousYVelocity = 0f;

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
        _wasGrounded = _isGrounded;
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _jumpGroundCheckDistance);

        HandleJump();
        HandlePeakGravity();

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
        
        _wasJumpHeld = _jumpHeld;
        _jumpHeld = Input.GetKey(_playerData.JumpKey);
        
        //Debug.Log($"Jump Debug - Held: {_jumpHeld}, WasHeld: {_wasJumpHeld}, Grounded: {_isGrounded}, IsJumping: {_isJumping}, Timer: {_jumpHoldTimer}");
    }

    void HandleJump()
    {
        // Detect jump start (when jump key is first pressed)
        bool jumpStarted = _jumpHeld && !_wasJumpHeld;

        // Handle jump start
        if (jumpStarted && _isGrounded && !_isJumping)
        {
            StartJump();
        }
        
        // Continue applying force while jump is held and within time limit
        if (_isJumping && _jumpHeld && _jumpHoldTimer < _playerData.JumpHoldTime)
        {
            ContinueJump();
            
            // After applying force, check if still grounded
            bool stillGrounded = Physics.Raycast(transform.position, Vector3.down, _jumpGroundCheckDistance);
            
            // If no longer touching ground after force application, we've truly left the ground
            if (!stillGrounded)
            {
                // Now we can start checking for jump end conditions
                CheckJumpEnd();
            }
        }
        else if (_isJumping && !_isGrounded)
        {
            // If we're jumping but not holding key, still check for jump end
            CheckJumpEnd();
        }
    }
    
    void CheckJumpEnd()
    {
        // Stop jumping when key released or timer expires or max height reached
        if (!_jumpHeld || _jumpHoldTimer >= _playerData.JumpHoldTime || 
            transform.position.y >= _jumpStartY + _playerData.MaxJumpHeight)
        {
            _isJumping = false;
        }
    }
    
    void HandlePeakGravity()
    {
        float currentYVelocity = _rb.linearVelocity.y;
        bool wasAtPeak = _isAtPeak;
        
        // Detect approaching peak (when velocity is slowing down significantly)
        bool approachingPeak = !_isGrounded && 
                              currentYVelocity > 0 && 
                              currentYVelocity <= _playerData.EarlyPeakDetectionVelocity;
        
        // Detect actual peak (when velocity changes from positive to negative or is very small)
        _isAtPeak = !_isGrounded && 
                   (Mathf.Abs(currentYVelocity) <= _playerData.PeakDetectionThreshold ||
                    (_previousYVelocity > 0 && currentYVelocity <= 0));
        
        // Start increasing gravity when approaching peak (prevents floating)
        if (approachingPeak || _isAtPeak)
        {
            if (!wasAtPeak)
            {
                _currentGravityScale = _playerData.PeakGravityScale;
            }
        }
        // If we're airborne and falling with good speed, normalize gravity
        else if (!_isGrounded && currentYVelocity < -1f)
        {
            _currentGravityScale = Mathf.Lerp(_currentGravityScale, 1f, _playerData.GravityNormalizationSpeed * Time.fixedDeltaTime);
        }
        
        // Reset gravity scale when grounded
        if (_isGrounded)
        {
            _currentGravityScale = 1f;
            _isAtPeak = false;
        }
        
        // Apply gravity scale to rigidbody
        _rb.useGravity = true;
        Physics.gravity = new Vector3(0, -9.81f * _currentGravityScale, 0);
        
        // Store current velocity for next frame comparison
        _previousYVelocity = currentYVelocity;
    }

    private void LateUpdate()
    {
        // Reset jump timer only when LANDING (transitioning from airborne to grounded)
        if (_isGrounded && !_wasGrounded && _rb.linearVelocity.y <= 0.1f)
        {
            _jumpHoldTimer = 0f;
        }
    }

    void StartJump()
    {
        _jumpStartY = transform.position.y;
        _isJumping = true;
        _jumpHoldTimer = 0f;
        //_animator.SetTrigger("Jump");
    }

    void ContinueJump()
    {
        _jumpHoldTimer += Time.fixedDeltaTime;
        
        // Calculate target velocity based on hold time (longer hold = higher velocity)
        float timeProgress = _jumpHoldTimer / _playerData.JumpHoldTime;
        float targetJumpVelocity = Mathf.Lerp(_playerData.MinJumpVelocity, _playerData.MaxJumpVelocity, timeProgress);
        
        // Set upward velocity directly (preserve horizontal movement)
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, targetJumpVelocity, _rb.linearVelocity.z);
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