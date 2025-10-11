using UnityEngine;

public class LootingController : MonoBehaviour
{
    [SerializeField] private float lootingSpeed = 1f;

    private bool isLooting = false;
    private float lootProgress = 0f;
    private int quantity = 0;

    public bool IsLooting => isLooting;
    public float LootProgress => lootProgress;
    public float LootingSpeed => lootingSpeed;
    public int Quantity => quantity;
    public bool IsEmpty => quantity <= 0;

    private void Update()
    {
        if (isLooting)
        {
            UpdateLootingProgress();
        }
    }

    public void SetQuantity(int newQuantity)
    {
        quantity = Mathf.Max(0, newQuantity);
    }

    public void StartLooting()
    {
        if (isLooting || IsEmpty) return;

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
        quantity--;
        lootProgress = 0f;
        GameEventsBus.Instance.Publish(new Looted());

        if (quantity > 0)
        {
            lootProgress = 0f;
        }
        else
        {
            isLooting = false;
        }
    }
}
