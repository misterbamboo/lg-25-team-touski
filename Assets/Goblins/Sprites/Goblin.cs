using UnityEngine;

public class Goblin : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float movementThreshold = 0.001f;

    private Vector3 lastPosition;
    private bool isMoving = false;

    private void Start()
    {
        lastPosition = transform.position;

        // Initialize animator to not moving state
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
    }

    private void Update()
    {
        CheckMovement();
    }

    private void CheckMovement()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        float threshold = movementThreshold * Time.deltaTime;
        bool currentlyMoving = distance > threshold;

        // Only update animator if movement state changed
        if (currentlyMoving != isMoving)
        {
            isMoving = currentlyMoving;
            UpdateAnimator();
        }

        lastPosition = transform.position;
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
        }
    }
}
