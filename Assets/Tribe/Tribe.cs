using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages a goblin tribe, spawning goblins around a central campfire
/// SRP: Single responsibility for tribe management and goblin spawning
/// </summary>
public class Tribe : MonoBehaviour
{
    [Header("Goblin Settings")]
    [SerializeField] private GameObject goblinPrefab;
    [SerializeField] private int minMemberCount = 3;
    [SerializeField] private int maxMemberCount = 8;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private float minDistanceBetweenGoblins = 1.5f;

    private List<GameObject> tribeMembers = new List<GameObject>();
    private int memberCount;

    public int GetMemberCount() => memberCount;
    public Vector3 GetTribeCenter() => transform.position;
    public List<GameObject> GetTribeMembers() => new List<GameObject>(tribeMembers);

    public void Initialize(int count)
    {
        memberCount = Mathf.Clamp(count, minMemberCount, maxMemberCount);
        SpawnGoblins();
    }

    private void Start()
    {
        if (memberCount == 0)
        {
            memberCount = Random.Range(minMemberCount, maxMemberCount + 1);
            SpawnGoblins();
        }
    }

    private void SpawnGoblins()
    {
        if (goblinPrefab == null)
        {
            Debug.LogError("Goblin prefab is not assigned to Tribe!");
            return;
        }

        for (int i = 0; i < memberCount; i++)
        {
            Vector3 spawnPosition = FindValidSpawnPosition();
            SpawnGoblin(spawnPosition);
        }
    }

    private Vector3 FindValidSpawnPosition()
    {
        int maxAttempts = 30;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 potentialPosition = GetRandomPositionInRadius();

            if (IsPositionValid(potentialPosition))
            {
                return potentialPosition;
            }
        }

        // Fallback: return random position even if not ideal
        return GetRandomPositionInRadius();
    }

    private Vector3 GetRandomPositionInRadius()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        return transform.position + new Vector3(randomCircle.x, randomCircle.y, 0f);
    }

    private bool IsPositionValid(Vector3 position)
    {
        // Check distance from campfire center
        if (Vector3.Distance(position, transform.position) < 1f)
        {
            return false;
        }

        // Check distance from other goblins
        foreach (GameObject goblin in tribeMembers)
        {
            if (goblin != null && Vector3.Distance(position, goblin.transform.position) < minDistanceBetweenGoblins)
            {
                return false;
            }
        }

        return true;
    }

    private void SpawnGoblin(Vector3 position)
    {
        GameObject goblin = Instantiate(goblinPrefab, position, Quaternion.identity, transform);
        tribeMembers.Add(goblin);
    }

    public void CleanupDeadMembers()
    {
        tribeMembers.RemoveAll(goblin => goblin == null);
    }

    public int GetAliveMemberCount()
    {
        CleanupDeadMembers();
        return tribeMembers.Count;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw spawn radius
        Gizmos.color = Color.yellow;
        DrawCircle(transform.position, spawnRadius, 32);

        // Draw minimum distance from center
        Gizmos.color = Color.red;
        DrawCircle(transform.position, 1f, 16);

        // Draw connections to tribe members
        if (Application.isPlaying && tribeMembers.Count > 0)
        {
            Gizmos.color = Color.green;
            foreach (GameObject goblin in tribeMembers)
            {
                if (goblin != null)
                {
                    Gizmos.DrawLine(transform.position, goblin.transform.position);
                }
            }
        }
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
