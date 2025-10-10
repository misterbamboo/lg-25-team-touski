using UnityEngine;

public class BagComponent : MonoBehaviour
{
    [SerializeField] float money = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeMoney(float change)
    {
        money += change;
        gameObject.transform.localScale = Vector3.one * (Mathf.Log(money * 2) + 1);
        gameObject.transform.localPosition = new Vector3(0, (-gameObject.transform.localScale.y / 2) - 0.6f, 0);
    }

    public float GetMoney() => money;
}
