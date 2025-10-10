using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LootingUIController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI lootText;
    [SerializeField] private LootingController lootingController;

    private bool playerInRange = false;

    private void Start()
    {
        InitializeUI();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        UpdateSliderValue();
    }

    public bool IsUIVisible() => slider.gameObject.activeSelf || lootText.gameObject.activeSelf;

    public void SetPlayerInRange(bool inRange)
    {
        playerInRange = inRange;

        if (inRange)
        {
            ShowTextIfNotLooting();
        }
        else
        {
            HideAllUI();
        }
    }

    private void InitializeUI()
    {
        slider.gameObject.SetActive(false);
        lootText.gameObject.SetActive(false);
    }

    private void SubscribeToEvents()
    {
        GameEventsBus.Instance.Subscribe<Looting>(OnLootingStarted);
        GameEventsBus.Instance.Subscribe<LootingInterupted>(OnLootingInterrupted);
        GameEventsBus.Instance.Subscribe<Looted>(OnLooted);
    }

    private void UnsubscribeFromEvents()
    {
        GameEventsBus.Instance.Unsubscribe<Looting>(OnLootingStarted);
        GameEventsBus.Instance.Unsubscribe<LootingInterupted>(OnLootingInterrupted);
        GameEventsBus.Instance.Unsubscribe<Looted>(OnLooted);
    }

    private void OnLootingStarted(Looting evt)
    {
        ShowSlider();
        HideText();
    }

    private void OnLootingInterrupted(LootingInterupted evt)
    {
        HideSlider();
        ShowTextIfInRange();
    }

    private void OnLooted(Looted evt)
    {
        HideSlider();
        ShowTextIfInRange();
    }

    private void UpdateSliderValue()
    {
        if (lootingController != null && slider.gameObject.activeSelf)
        {
            slider.value = lootingController.LootProgress;
        }
    }

    private void ShowSlider()
    {
        slider.gameObject.SetActive(true);
    }

    private void HideSlider()
    {
        slider.gameObject.SetActive(false);
        slider.value = 0f;
    }

    private void ShowText()
    {
        lootText.gameObject.SetActive(true);
    }

    private void HideText()
    {
        lootText.gameObject.SetActive(false);
    }

    private void ShowTextIfInRange()
    {
        if (playerInRange)
        {
            ShowText();
        }
    }

    private void ShowTextIfNotLooting()
    {
        if (lootingController != null && !lootingController.IsLooting)
        {
            ShowText();
        }
    }

    private void HideAllUI()
    {
        HideSlider();
        HideText();
    }
}
