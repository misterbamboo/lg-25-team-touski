using UnityEngine;

public class ObjectPoolComponent : MonoBehaviour
{
    [SerializeField] GameObject objectToPool;
    [SerializeField] int poolsize = 20;

    ObjectPool<GameObject> pool = new ObjectPool<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Initialize();
    }

    public GameObject GetObject => pool.Take();
    public void PutObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Add(obj);
    }

    public void Initialize()
    {
        for (int i = 0; i != poolsize; i++)
        {
            var clone = Instantiate(objectToPool, transform);
            clone.GetComponent<IPoolable>().Pool = this;
            clone.SetActive(false);
            pool.Add(clone);
        }
    }
}
