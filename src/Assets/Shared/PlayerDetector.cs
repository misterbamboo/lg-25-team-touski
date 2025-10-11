using System;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private const string PlayerTag = "Player";

    public event Action OnPlayerEnter;
    public event Action OnPlayerExit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag))
        {
            OnPlayerEnter?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag))
        {
            OnPlayerExit?.Invoke();
        }
    }
}
