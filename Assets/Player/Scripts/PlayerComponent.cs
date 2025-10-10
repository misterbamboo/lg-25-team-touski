using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PlayerComponent : MonoBehaviour
{
    [SerializeField] float speed = 3;
    [SerializeField] float atkCd = 2;
    [SerializeField] float slowMoveCd = 2;
    [SerializeField] float blinkSpeed = 0.2f;
    [SerializeField] PlayerSpriteComponent sprite;
    [SerializeField] BagComponent bag;
    [SerializeField] BoxCollider2D attackBox;
    public float damage = 1;
    bool iFrames = false;
    float atkTimer = 0;
    float slowMoveTimer = 0;
    Direction dir;
    public bool isDead = false;
    public Vector2 move = Vector2.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        GameEventsBus.Instance.Subscribe<Looted>((l) => MoneyUp(1));
    }

    private void Start()
    {
        MoneyUp(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            atkTimer += Time.deltaTime;
            slowMoveTimer += Time.deltaTime;
            Movement();
        }
    }

    private void Movement()
    {
        if (bag.GetMoney() < 40)
        {
            transform.Translate((speed * move) * Time.deltaTime, Space.World);
            switch (dir)
            {
                case Direction.Left:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    break;
                case Direction.Right:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                    break;
                case Direction.Up:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    break;
                case Direction.Down:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                    break;
            }
        }
        else
        {
            if (slowMoveTimer >= slowMoveCd)
            {
                StartCoroutine(SlowMovement());
                slowMoveTimer = 0;
            }
        }
        Camera.main.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10);
    }

    IEnumerator SlowMovement()
    {
        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            transform.Translate((0.7f * move) * Time.deltaTime, Space.World); 
            switch (dir)
            {
                case Direction.Left:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    break;
                case Direction.Right:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                    break;
                case Direction.Up:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    break;
                case Direction.Down:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                    break;
            }
            yield return null;
        }
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        if (!isDead)
        {
            move = ctx.ReadValue<Vector2>();

            if (move.x == 1)
                dir = Direction.Right;
            else if (move.x == -1)
                dir = Direction.Left;
            else if (move.y == 1)
                dir = Direction.Up;
            else if (move.y == -1)
                dir = Direction.Down;
        }
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        if (atkTimer >= atkCd)
        {
            Debug.Log("atk");
            StartCoroutine(Swing());
            atkTimer = 0;
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(Mathf.Log((bag.GetMoney() + 3) * 0.3f));
        attackBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        attackBox.gameObject.SetActive(false);
    }

    public void Damage()
    {
        if (!iFrames && !isDead)
        {
            MoneyUp(-1);
            if (bag.GetMoney() <= 0)
            {
                gameObject.SetActive(false);
            }
            else
                StartCoroutine(Flash());
        }
    }

    IEnumerator Flash()
    {
        iFrames = true;
        float time = 0;
        while (time < 2)
        {
            sprite.ChangeColor(Color.red);
            time += blinkSpeed;
            yield return new WaitForSeconds(blinkSpeed);
            sprite.ChangeColor(new Color(0.3f, 0, 0));
            time += blinkSpeed;
            yield return new WaitForSeconds(blinkSpeed);
        }
        sprite.ChangeColor(Color.white);
        iFrames = false;
        yield return null;
    }

    public void MoneyUp(float money)
    {
        bag.ChangeMoney(money);
        speed = (Mathf.Pow(bag.GetMoney() + 3, -1) * 15) + 0.7f;
    }
}


public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
