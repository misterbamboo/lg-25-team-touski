using UnityEngine;

public class BagCollisions : MonoBehaviour
{
    BagComponent bag;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyAttack"))
        {
            bag.ChangeMoney(-1);
        }
    }
}
