using UnityEngine;

/// <summary>
/// Represents a campfire at the center of a tribe
/// Can be extended later with fire effects, warmth radius, etc.
/// </summary>
public class Campfire : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private SpriteRenderer fireSprite;
    [SerializeField] private Color fireColor = new Color(1f, 0.5f, 0f, 1f);

    private void Start()
    {
        InitializeVisuals();
    }

    // Command: Initialize campfire visuals (SRP: Visual setup)
    private void InitializeVisuals()
    {
        if (fireSprite != null)
        {
            fireSprite.color = fireColor;
        }
    }

    // Query: Get campfire position (CQS: Pure query)
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    // Gizmos for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
