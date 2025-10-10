using UnityEngine;

public class LootingController : MonoBehaviour
{
    [SerializeField] private float lootingSpeed = 1f;

    private bool isLooting = false;
    private float lootProgress = 0f;

    public bool IsLooting => isLooting;
    public float LootProgress => lootProgress;
    public float LootingSpeed => lootingSpeed;

    private void Update()
    {
        if (isLooting)
        {
            UpdateLootingProgress();
        }
    }

    public void StartLooting()
    {
        if (isLooting) return;

        isLooting = true;
        lootProgress = 0f;
        GameEventsBus.Instance.Publish(new Looting());
    }

    public void StopLooting()
    {
        if (!isLooting) return;

        isLooting = false;
        lootProgress = 0f;
        GameEventsBus.Instance.Publish(new LootingInterupted());
    }

    private void UpdateLootingProgress()
    {
        lootProgress += Time.deltaTime / lootingSpeed;

        if (lootProgress >= 1f)
        {
            CompleteLoot();
        }
    }

    private void CompleteLoot()
    {
        isLooting = false;
        lootProgress = 0f;
        GameEventsBus.Instance.Publish(new Looted());
    }
}
