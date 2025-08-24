using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Movement Settings")]
    public float Speed = 6f;
    public float RunSpeed = 12f;
    public float RotationSpeed = 5f;
    public bool CanMove = true;
    
    [Header("Jump Settings")]
    public float MinJumpVelocity = 5f;
    public float MaxJumpVelocity = 15f;
    public float JumpHoldTime = 0.3f;
    public float MaxJumpHeight = 3f;
    public float MaxDownwardVelocity = -15f;
    public KeyCode JumpKey = KeyCode.Space;
    
    [Header("Peak Gravity Settings")]
    public float PeakGravityScale = 4f;
    public float PeakDetectionThreshold = 2f;
    public float GravityNormalizationSpeed = 2f;
    public float EarlyPeakDetectionVelocity = 3f;
    
    [Header("Health/Status Settings")]
    public int MaxHealth = 100;
    public int StartingHealth = 100;
    
    [Header("Battery Settings")]
    public float BatteryLife = 1000f;
    public float IdleDrain = 1f;
    public float MoveDrain = 3f;
    public float SprintDrain = 7f;
    public float SprintThreshold = 5f;
    
    // Runtime stats that can be modified during gameplay
    [System.NonSerialized]
    public int CurrentHealth;
    [System.NonSerialized]
    public float CurrentBatteryLife;
    
    private void OnEnable()
    {
        ResetRuntimeStats();
    }
    
    public void ResetRuntimeStats()
    {
        CurrentHealth = StartingHealth;
        CurrentBatteryLife = BatteryLife;
    }
}