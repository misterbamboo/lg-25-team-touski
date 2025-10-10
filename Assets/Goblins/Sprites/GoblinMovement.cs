using UnityEngine;

/// <summary>
/// Handles Goblin AI movement behavior
/// - Wanders when idle
/// - Stays near spawn point
/// - Chases player when in range
/// - Returns to last known player position when player escapes
/// </summary>
public class GoblinMovement : MonoBehaviour
{
    private const string PlayerTag = "Player";
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 4f;
    [SerializeField] private float accelerationRate = 3f;
    [SerializeField] private float decelerationRate = 3f;

    [Header("Wander Settings")]
    [SerializeField] private float idleTimeBeforeWander = 2f;
    [SerializeField] private float minWanderDistance = 3f;
    [SerializeField] private float maxWanderDistance = 5f;
    [SerializeField] private float maxDistanceFromSpawn = 5f;
    [SerializeField] private float stoppingDistance = 0.5f;

    [Header("Player Detection")]
    [SerializeField] private PlayerDetector playerDetector;

    private Vector3 spawnPoint;
    private Vector3 targetPosition;
    private Vector3 lastKnownPlayerPosition;
    private Transform playerTransform;

    private float idleTimer = 0f;
    private float currentSpeed = 0f;
    private float targetSpeed = 0f;
    private bool isChasing = false;
    private bool hasLastKnownPlayerPosition = false;
    private bool playerInRange = false;
    private float distanceToTarget = 0f;

    public enum GoblinState
    {
        Idle,
        Wandering,
        ChasingPlayer,
        GoingToLastKnownPosition
    }

    private GoblinState currentState = GoblinState.Idle;

    private void Start()
    {
        spawnPoint = transform.position;
        targetPosition = transform.position;

        if (playerDetector != null)
        {
            playerDetector.OnPlayerEnter += OnPlayerEnterRange;
            playerDetector.OnPlayerExit += OnPlayerExitRange;
        }
    }

    private void OnDestroy()
    {
        if (playerDetector != null)
        {
            playerDetector.OnPlayerEnter -= OnPlayerEnterRange;
            playerDetector.OnPlayerExit -= OnPlayerExitRange;
        }
    }

    private void OnPlayerEnterRange()
    {
        playerInRange = true;
        
        GameObject playerObj = FindPlayer();
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private static GameObject FindPlayer()
    {
        return GameObject.FindGameObjectWithTag(PlayerTag);
    }

    private void OnPlayerExitRange()
    {
        playerInRange = false;
        StoreLastKnownPosition();
    }

    private void StoreLastKnownPosition()
    {
        if (playerTransform != null)
        {
            lastKnownPlayerPosition = playerTransform.position;
            hasLastKnownPlayerPosition = true;
        }
    }

    private void Update()
    {
        UpdateState();
        MoveTowardsTarget();
    }

    private void UpdateState()
    {
        distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // Check if player is in detection range
        if (playerInRange && playerTransform != null)
        {
            currentState = GoblinState.ChasingPlayer;
            isChasing = true;
            lastKnownPlayerPosition = playerTransform.position;
            hasLastKnownPlayerPosition = true;
            targetPosition = playerTransform.position;
            targetSpeed = runSpeed;
        }
        // Player escaped, go to last known position
        else if (hasLastKnownPlayerPosition && currentState == GoblinState.ChasingPlayer)
        {
            currentState = GoblinState.GoingToLastKnownPosition;
            targetPosition = lastKnownPlayerPosition;
            targetSpeed = walkSpeed;
        }
        // Arrived at last known position or no player detected
        else if (currentState == GoblinState.GoingToLastKnownPosition && distanceToTarget < stoppingDistance)
        {
            hasLastKnownPlayerPosition = false;
            currentState = GoblinState.Idle;
            idleTimer = 0f;
            targetSpeed = 0f;
        }
        // Idle and wandering behavior
        else if (!isChasing)
        {
            if (currentState == GoblinState.Idle)
            {
                idleTimer += Time.deltaTime;
                targetSpeed = 0f;

                if (idleTimer >= idleTimeBeforeWander)
                {
                    SetNewWanderTarget();
                    currentState = GoblinState.Wandering;
                    targetSpeed = walkSpeed;
                }
            }
            else if (currentState == GoblinState.Wandering)
            {
                // Start decelerating when approaching target
                float decelerationDistance = (currentSpeed * currentSpeed) / (2f * decelerationRate);

                if (distanceToTarget <= decelerationDistance)
                {
                    targetSpeed = 0f;
                }
                else
                {
                    targetSpeed = walkSpeed;
                }

                // Reached target, go back to idle
                if (distanceToTarget < stoppingDistance)
                {
                    currentState = GoblinState.Idle;
                    idleTimer = 0f;
                    targetSpeed = 0f;
                    currentSpeed = 0f;
                }
            }
        }

        // Reset chasing flag if not actively chasing
        if (currentState != GoblinState.ChasingPlayer)
        {
            isChasing = false;
        }

        // Update current speed with acceleration/deceleration
        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        if (currentSpeed < targetSpeed)
        {
            // Accelerate
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelerationRate * Time.deltaTime);
        }
        else if (currentSpeed > targetSpeed)
        {
            // Decelerate
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, decelerationRate * Time.deltaTime);
        }
    }

    private void MoveTowardsTarget()
    {
        if (currentSpeed > 0.01f && Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * currentSpeed * Time.deltaTime;
        }
    }

    private void SetNewWanderTarget()
    {
        // Random distance between min and max wander distance
        float randomDistance = Random.Range(minWanderDistance, maxWanderDistance);

        // Random direction
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0; // Keep on same Y plane
        randomDirection.Normalize();

        Vector3 potentialTarget = transform.position + randomDirection * randomDistance;

        // Check if too far from spawn point
        float distanceFromSpawn = Vector3.Distance(potentialTarget, spawnPoint);

        if (distanceFromSpawn > maxDistanceFromSpawn)
        {
            // Set target towards spawn point instead
            Vector3 directionToSpawn = (spawnPoint - transform.position).normalized;
            potentialTarget = transform.position + directionToSpawn * randomDistance;
        }

        targetPosition = potentialTarget;
    }

    // Query methods (CQS)
    public bool IsMoving() => currentSpeed > 0.01f;
    public float GetCurrentSpeed() => currentSpeed;
    public GoblinState GetCurrentState() => currentState;
    public bool IsPlayerInRange() => playerInRange;

    // Gizmos for debugging
    private void OnDrawGizmosSelected()
    {
        // Draw spawn point
        Vector3 spawn = Application.isPlaying ? spawnPoint : transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawn, 0.5f);

        // Draw max distance from spawn
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawn, maxDistanceFromSpawn);

        // Draw target position
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawSphere(targetPosition, 0.3f);

            // Draw last known player position if exists
            if (hasLastKnownPlayerPosition)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(lastKnownPlayerPosition, 0.5f);
            }
        }
    }
}
