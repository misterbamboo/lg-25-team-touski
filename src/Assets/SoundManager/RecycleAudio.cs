using UnityEngine;

public class RecycleAudio : MonoBehaviour, IPoolable
{
    public ObjectPoolComponent Pool { get; set; }

    float elapsed = 0;
    float soundTime;
    public void Recycle(float time)
    {
        elapsed = 0;
        soundTime = time;
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > soundTime)
        {
            Pool.PutObject(gameObject);
        }
    }
}
