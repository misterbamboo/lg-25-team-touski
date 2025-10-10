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

    [Header("Sprite Swapping")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hitSprite;
    [SerializeField] private float hitSpriteDuration = 1f;

    private Vector3 lastPosition;
    private bool isWalking = false;
    private float currentSpeed = 0f;
    private float hitSpriteTimer = 0f;
    private bool isShowingHitSprite = false;

    private void Start()
    {
        InitializePosition();
        InitializeAnimator();
        InitializeSprite();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        CheckMovement();
        UpdateAnimatorSpeed();
        UpdateHitSpriteTimer();
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

    // Command: Initialize sprite (SRP: Sprite initialization)
    private void InitializeSprite()
    {
        if (spriteRenderer != null && defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
            isShowingHitSprite = false;
            hitSpriteTimer = 0f;
        }
    }

    // Command: Subscribe to game events (SRP: Event subscription)
    private void SubscribeToEvents()
    {
        GameEventsBus.Instance.Subscribe<PlayerDamaged>(OnPlayerDamaged);
    }

    // Command: Unsubscribe from events (SRP: Event cleanup)
    private void UnsubscribeFromEvents()
    {
        GameEventsBus.Instance.Unsubscribe<PlayerDamaged>(OnPlayerDamaged);
    }

    // Command: Handle player damaged event (SRP: Damage response)
    private void OnPlayerDamaged(PlayerDamaged evt)
    {
        ShowHitSprite();
    }

    // Command: Show hit sprite (SRP: Hit sprite display)
    private void ShowHitSprite()
    {
        if (spriteRenderer != null && hitSprite != null)
        {
            spriteRenderer.sprite = hitSprite;
            isShowingHitSprite = true;
            hitSpriteTimer = hitSpriteDuration;
        }
    }

    // Command: Update hit sprite timer (SRP: Timer management)
    private void UpdateHitSpriteTimer()
    {
        if (isShowingHitSprite)
        {
            hitSpriteTimer -= Time.deltaTime;

            if (hitSpriteTimer <= 0f)
            {
                RevertToDefaultSprite();
            }
        }
    }

    // Command: Revert to default sprite (SRP: Sprite restoration)
    private void RevertToDefaultSprite()
    {
        if (spriteRenderer != null && defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
            isShowingHitSprite = false;
            hitSpriteTimer = 0f;
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
