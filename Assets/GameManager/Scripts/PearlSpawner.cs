using UnityEngine;

public class PearlSpawner : MonoBehaviour
{
    [SerializeField] ObjectPoolComponent pool;
    public static PearlSpawner Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetPearl(Transform transform)
    {
        GameObject pearl = pool.GetObject;
        pearl.transform.position = transform.position;
        pearl.gameObject.SetActive(true);
    }
}
