using UnityEngine;

/// <summary>
/// Controls player animator based on movement
/// </summary>
public class PlayerAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float movementThreshold = 0.01f;

    private Vector3 lastPosition;
    private bool isWalking = false;

    private void Start()
    {
        InitializePosition();
        InitializeAnimator();
    }

    private void Update()
    {
        CheckMovement();
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

        // Only update animator if walking state changed
        if (currentlyWalking != isWalking)
        {
            isWalking = currentlyWalking;
            UpdateAnimator();
        }

        lastPosition = transform.position;
    }

    private float CalculateDistanceMoved()
    {
        return Vector3.Distance(transform.position, lastPosition);
    }

    private bool IsMoving(float distance)
    {
        float threshold = movementThreshold * Time.deltaTime;
        return distance > threshold;
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("IsWalking", isWalking);
        }
    }

    public bool IsPlayerWalking() => isWalking;
}
