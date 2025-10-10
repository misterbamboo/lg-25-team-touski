using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    PlayerComponent player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Treasure"))
        {
            player.MoneyUp(1);
            collision.gameObject.GetComponent<TreasureComponent>().IsPicked();
        }

        if (collision.gameObject.CompareTag("EnemyAttack"))
        {
            player.Damage();
        }
    }
}
