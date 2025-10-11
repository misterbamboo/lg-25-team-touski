using UnityEngine;

public class PlayerSpriteComponent : MonoBehaviour
{
    [SerializeField] PlayerComponent player;
    SpriteRenderer spriteRenderer;
    private float lastMoveX = 0;
    private float lastMoveY = -1;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;

        Vector2 move = player.move;

        bool isMoving = move.magnitude > 0.1f;

        if (isMoving)
        {

            lastMoveX = move.x;
            lastMoveY = move.y;
        }
        else
        {

        }
    }

    public void ChangeColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
}
