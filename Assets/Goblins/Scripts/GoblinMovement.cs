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
    [SerializeField] private float walkAccelerationRate = 1f;
    [SerializeField] private float walkDecelerationRate = 1.5f;
    [SerializeField] private float runAccelerationRate = 5f;
    [SerializeField] private float runDecelerationRate = 3f;
    [SerializeField] private Animator animator;

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
        GameEventsBus.Instance.Publish(new GoblinSurprise());
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
        CalculateDistanceToTarget();
        DetermineCurrentState();
        UpdateChasingFlag();
        UpdateSpeed();
        UpdateAnimatorSpeed();
    }

    private void CalculateDistanceToTarget()
    {
        distanceToTarget = Vector3.Distance(transform.position, targetPosition);
    }

    private void DetermineCurrentState()
    {
        if (ShouldChasePlayer())
        {
            TransitionToChasingPlayer();
        }
        else if (ShouldGoToLastKnownPosition())
        {
            TransitionToGoingToLastKnownPosition();
        }
        else if (ShouldReturnToIdle())
        {
            TransitionToIdle();
        }
        else if (IsIdleOrWandering())
        {
            HandleIdleAndWanderingBehavior();
        }
    }

    private bool ShouldChasePlayer()
    {
        return playerInRange && playerTransform != null;
    }

    private bool ShouldGoToLastKnownPosition()
    {
        return hasLastKnownPlayerPosition && currentState == GoblinState.ChasingPlayer;
    }

    private bool ShouldReturnToIdle()
    {
        return currentState == GoblinState.GoingToLastKnownPosition && HasReachedTarget();
    }

    private bool HasReachedTarget()
    {
        return distanceToTarget < stoppingDistance;
    }

    private bool IsIdleOrWandering()
    {
        return !isChasing;
    }

    private void TransitionToChasingPlayer()
    {
        currentState = GoblinState.ChasingPlayer;
        isChasing = true;
        lastKnownPlayerPosition = playerTransform.position;
        hasLastKnownPlayerPosition = true;
        targetPosition = playerTransform.position;
        targetSpeed = runSpeed;
    }

    private void TransitionToGoingToLastKnownPosition()
    {
        currentState = GoblinState.GoingToLastKnownPosition;
        targetPosition = lastKnownPlayerPosition;
        targetSpeed = walkSpeed;
    }

    private void TransitionToIdle()
    {
        hasLastKnownPlayerPosition = false;
        currentState = GoblinState.Idle;
        idleTimer = 0f;
        targetSpeed = 0f;
    }

    private void HandleIdleAndWanderingBehavior()
    {
        if (currentState == GoblinState.Idle)
        {
            UpdateIdleState();
        }
        else if (currentState == GoblinState.Wandering)
        {
            UpdateWanderingState();
        }
    }

    private void UpdateIdleState()
    {
        idleTimer += Time.deltaTime;
        targetSpeed = 0f;

        if (ShouldStartWandering())
        {
            StartWandering();
        }
    }

    private bool ShouldStartWandering()
    {
        return idleTimer >= idleTimeBeforeWander;
    }

    private void StartWandering()
    {
        SetNewWanderTarget();
        currentState = GoblinState.Wandering;
        targetSpeed = walkSpeed;
    }

    private void UpdateWanderingState()
    {
        UpdateWanderingSpeed();

        if (HasReachedTarget())
        {
            StopWandering();
        }
    }

    private void UpdateWanderingSpeed()
    {
        float decelerationDistance = CalculateDecelerationDistance();

        if (ShouldDecelerate(decelerationDistance))
        {
            targetSpeed = 0f;
        }
        else
        {
            targetSpeed = walkSpeed;
        }
    }

    private float CalculateDecelerationDistance()
    {
        float decelRate = GetCurrentDecelerationRate();
        return (currentSpeed * currentSpeed) / (2f * decelRate);
    }

    private bool ShouldDecelerate(float decelerationDistance)
    {
        return distanceToTarget <= decelerationDistance;
    }

    private void StopWandering()
    {
        currentState = GoblinState.Idle;
        idleTimer = 0f;
        targetSpeed = 0f;
        currentSpeed = 0f;
    }

    private void UpdateChasingFlag()
    {
        if (currentState != GoblinState.ChasingPlayer)
        {
            isChasing = false;
        }
    }

    private void UpdateSpeed()
    {
        float accelerationRate = GetCurrentAccelerationRate();
        float decelerationRate = GetCurrentDecelerationRate();

        if (currentSpeed < targetSpeed)
        {
            // Accelerate with smooth exponential easing
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, accelerationRate * Time.deltaTime);
        }
        else if (currentSpeed > targetSpeed)
        {
            // Decelerate with smooth exponential easing
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, decelerationRate * Time.deltaTime);
        }
    }

    private float GetCurrentAccelerationRate()
    {
        return IsChasing() ? runAccelerationRate : walkAccelerationRate;
    }

    private float GetCurrentDecelerationRate()
    {
        return IsChasing() ? runDecelerationRate : walkDecelerationRate;
    }

    private bool IsChasing()
    {
        return currentState == GoblinState.ChasingPlayer;
    }

    private void UpdateAnimatorSpeed()
    {
        if (animator != null)
        {
            float normalizedSpeed = CalculateNormalizedSpeed();
            animator.speed = normalizedSpeed;
        }
    }

    private float CalculateNormalizedSpeed()
    {
        float maxSpeed = Mathf.Max(walkSpeed, runSpeed);
        return currentSpeed / maxSpeed;
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
        float randomDistance = Random.Range(minWanderDistance, maxWanderDistance);

        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0;
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

    public bool IsMoving() => currentSpeed > 0.01f;
    public float GetCurrentSpeed() => currentSpeed;
    public GoblinState GetCurrentState() => currentState;
    public bool IsPlayerInRange() => playerInRange;
}
