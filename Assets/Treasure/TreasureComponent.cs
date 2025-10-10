using Unity.VisualScripting;
using UnityEngine;

public class TreasureComponent : MonoBehaviour
{
    public ObjectPoolComponent Pool;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void IsPicked()
    {
        if (Pool != null)
            Pool.PutObject(gameObject);
        else
            Destroy(gameObject);
    }
}
