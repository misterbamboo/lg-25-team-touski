using UnityEngine;
using UnityEngine.UI;

public class Chess : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PlayerDetector playerDetector;

    private void Start()
    {
        slider.gameObject.SetActive(false);

        playerDetector.OnPlayerEnter += () => OnPlayerRangeChanged(true);
        playerDetector.OnPlayerExit += () => OnPlayerRangeChanged(false);
    }

    private void OnPlayerRangeChanged(bool inRange)
    {
        slider.gameObject.SetActive(inRange);
    }
}
