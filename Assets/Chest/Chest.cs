using UnityEngine;

/// <summary>
/// Coordinates looting system components (SRP: Single responsibility for coordination)
/// DIP: Depends on abstractions (components), not concrete implementations
/// OCP: Open for extension, closed for modification
/// </summary>
public class Chess : MonoBehaviour
{
    [SerializeField] private PlayerDetector playerDetector;
    [SerializeField] private LootingInputHandler inputHandler;
    [SerializeField] private LootingUIController uiController;
    [SerializeField] private LootingController lootingController;
    [SerializeField] private int initialQuantity = 5;

    private void Start()
    {
        InitializeLootingQuantity();
        SubscribeToPlayerDetector();
    }

    private void InitializeLootingQuantity()
    {
        if (lootingController != null)
        {
            lootingController.SetQuantity(initialQuantity);
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromPlayerDetector();
    }

    private void SubscribeToPlayerDetector()
    {
        if (playerDetector != null)
        {
            playerDetector.OnPlayerEnter += OnPlayerEnter;
            playerDetector.OnPlayerExit += OnPlayerExit;
        }
    }

    private void UnsubscribeFromPlayerDetector()
    {
        if (playerDetector != null)
        {
            playerDetector.OnPlayerEnter -= OnPlayerEnter;
            playerDetector.OnPlayerExit -= OnPlayerExit;
        }
    }

    private void OnPlayerEnter()
    {
        if (inputHandler != null)
            inputHandler.SetPlayerInRange(true);

        if (uiController != null)
            uiController.SetPlayerInRange(true);
    }

    private void OnPlayerExit()
    {
        if (inputHandler != null)
            inputHandler.SetPlayerInRange(false);

        if (uiController != null)
            uiController.SetPlayerInRange(false);
    }
}
