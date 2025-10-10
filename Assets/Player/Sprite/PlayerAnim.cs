using UnityEngine;

/// <summary>
/// Controls player animator based on movement
/// SRP: Single responsibility for player animation
/// </summary>
public class PlayerAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float movementThreshold = 0.01f;
    [SerializeField] private float maxSpeed = 5f; // Maximum expected player speed

    private Vector3 lastPosition;
    private bool isWalking = false;
    private float currentSpeed = 0f;

    private void Start()
    {
        InitializePosition();
        InitializeAnimator();
    }

    private void Update()
    {
        CheckMovement();
        UpdateAnimatorSpeed();
    }

    private void InitializePosition()
    {
        lastPosition = transform.position;
    }

    private void InitializeAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private void CheckMovement()
    {
        float distance = CalculateDistanceMoved();
        bool currentlyWalking = IsMoving(distance);

        // Calculate current speed
        currentSpeed = CalculateCurrentSpeed(distance);

        // Only update animator if walking state changed
        if (currentlyWalking != isWalking)
        {
            isWalking = currentlyWalking;
            UpdateAnimator();
        }

        lastPosition = transform.position;
    }

    // Query: Calculate distance moved this frame (CQS: Pure calculation)
    private float CalculateDistanceMoved()
    {
        return Vector3.Distance(transform.position, lastPosition);
    }

    // Query: Calculate current movement speed (CQS: Pure calculation)
    private float CalculateCurrentSpeed(float distance)
    {
        if (Time.deltaTime > 0)
        {
            return distance / Time.deltaTime;
        }
        return 0f;
    }

    // Query: Check if player is moving (CQS: Pure query)
    private bool IsMoving(float distance)
    {
        float threshold = movementThreshold * Time.deltaTime;
        return distance > threshold;
    }

    // Command: Update animator walking state (SRP: Animator update)
    private void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", isWalking);
        }
    }

    // Command: Update animator speed based on movement (SRP: Speed sync)
    private void UpdateAnimatorSpeed()
    {
        if (animator != null)
        {
            float normalizedSpeed = CalculateNormalizedSpeed();
            animator.speed = normalizedSpeed;
        }
    }

    // Query: Calculate normalized speed for animator (CQS: Pure calculation)
    private float CalculateNormalizedSpeed()
    {
        if (maxSpeed > 0)
        {
            return Mathf.Clamp01(currentSpeed / maxSpeed);
        }
        return 1f;
    }

    // Query methods (CQS)
    public bool IsPlayerWalking() => isWalking;
    public float GetCurrentSpeed() => currentSpeed;
}
