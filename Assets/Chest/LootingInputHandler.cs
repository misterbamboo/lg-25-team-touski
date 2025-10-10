using UnityEngine;

public class LootingInputHandler : MonoBehaviour
{
    [SerializeField] private KeyCode lootKey = KeyCode.E;
    [SerializeField] private LootingController lootingController;

    private bool playerInRange = false;

    private void Update()
    {
        if (!playerInRange || lootingController == null)
            return;

        HandleLootingInput();
    }

    public void SetPlayerInRange(bool inRange)
    {
        playerInRange = inRange;

        if (!inRange && lootingController.IsLooting)
        {
            lootingController.StopLooting();
        }
    }

    private void HandleLootingInput()
    {
        if (Input.GetKeyDown(lootKey) && !lootingController.IsLooting)
        {
            lootingController.StartLooting();
        }
        else if (Input.GetKeyUp(lootKey) && lootingController.IsLooting)
        {
            lootingController.StopLooting();
        }
    }
}
