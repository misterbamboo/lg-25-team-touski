using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerComponent : MonoBehaviour
{
    [SerializeField] float speed = 3;
    [SerializeField] float atkCd = 2;
    [SerializeField] float slowMoveCd = 2;
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

    // Update is called once per frame
    void Update()
    {
        speed = (Mathf.Pow(bag.GetMoney(),-1) * 2) + 1;
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
            StartCoroutine(Swing());
            atkTimer = 0;
        }
    }

    IEnumerator Swing()
    {
        attackBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        attackBox.gameObject.SetActive(false);
    }

    public void Damage()
    {
        if (!iFrames && !isDead)
        {
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
        float timeElapsed = 0;
        iFrames = true;
        while (timeElapsed < 3)
        {
            sprite.gameObject.SetActive(!sprite.gameObject.activeSelf);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        iFrames = false;
        sprite.gameObject.SetActive(true);
        yield return null;
    }

    public void MoneyUp(float money)
    {
        bag.ChangeMoney(money);
    }
}


public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
