using UnityEngine;

/// <summary>
/// Manages house behavior including roof fading when player enters/exits
/// SRP: Single responsibility for house interactions
/// </summary>
public class House : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerDetector playerDetector;
    [SerializeField] private SpriteRenderer roofSprite;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float fadeOutAlpha = 0f;
    [SerializeField] private float fadeInAlpha = 1f;

    private bool isPlayerInside = false;
    private float currentAlpha;
    private float targetAlpha;
    private float fadeVelocity;

    private void Start()
    {
        InitializeRoof();
        SubscribeToPlayerDetector();
    }

    private void OnDestroy()
    {
        UnsubscribeFromPlayerDetector();
    }

    private void Update()
    {
        UpdateRoofAlpha();
    }

    // Command: Initialize roof alpha (SRP: Initialization)
    private void InitializeRoof()
    {
        if (roofSprite != null)
        {
            currentAlpha = fadeInAlpha;
            targetAlpha = fadeInAlpha;
            SetRoofAlpha(currentAlpha);

            // Activate roof after setting alpha (roof starts inactive in editor)
            roofSprite.gameObject.SetActive(true);
        }
    }

    // Command: Subscribe to player detector events (SRP: Event subscription)
    private void SubscribeToPlayerDetector()
    {
        if (playerDetector != null)
        {
            playerDetector.OnPlayerEnter += OnPlayerEnterHouse;
            playerDetector.OnPlayerExit += OnPlayerExitHouse;
        }
    }

    // Command: Unsubscribe from events (SRP: Event cleanup)
    private void UnsubscribeFromPlayerDetector()
    {
        if (playerDetector != null)
        {
            playerDetector.OnPlayerEnter -= OnPlayerEnterHouse;
            playerDetector.OnPlayerExit -= OnPlayerExitHouse;
        }
    }

    // Command: Handle player entering house (SRP: Enter logic)
    private void OnPlayerEnterHouse()
    {
        isPlayerInside = true;
        targetAlpha = fadeOutAlpha;
    }

    // Command: Handle player exiting house (SRP: Exit logic)
    private void OnPlayerExitHouse()
    {
        isPlayerInside = false;
        targetAlpha = fadeInAlpha;
    }

    // Command: Update roof alpha with smooth transition (SRP: Alpha animation)
    private void UpdateRoofAlpha()
    {
        if (roofSprite == null || Mathf.Approximately(currentAlpha, targetAlpha))
            return;

        // Smooth damp for organic easing
        currentAlpha = Mathf.SmoothDamp(currentAlpha, targetAlpha, ref fadeVelocity, fadeDuration);

        // Snap to target when very close
        if (Mathf.Abs(currentAlpha - targetAlpha) < 0.01f)
        {
            currentAlpha = targetAlpha;
        }

        SetRoofAlpha(currentAlpha);
    }

    // Command: Set roof sprite alpha (SRP: Alpha setting)
    private void SetRoofAlpha(float alpha)
    {
        if (roofSprite != null)
        {
            Color color = roofSprite.color;
            color.a = alpha;
            roofSprite.color = color;
        }
    }

    // Query methods (CQS)
    public bool IsPlayerInside() => isPlayerInside;
    public float GetCurrentRoofAlpha() => currentAlpha;
}
