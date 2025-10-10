using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

public class SoundManage : MonoBehaviour
{
    public static SoundManage Instance;

    [SerializeField] ObjectPoolComponent pool;
    [SerializeField] AudioClip slash;
    [SerializeField] AudioClip hurt;
    [SerializeField] AudioClip money;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        GameEventsBus.Instance.Subscribe<PlayerSlash>((l) => { PlaySFX(slash, PlayerComponent.playerTransform); });
        GameEventsBus.Instance.Subscribe<PlayerDamaged>((l) => { PlaySFX(hurt, PlayerComponent.playerTransform); });
    }

    public void PlaySFX(AudioClip clip, Transform sourceTransform)
    {
        GameObject audioObject = pool.GetObject;
        audioObject.SetActive(true);
        AudioSource audio = audioObject.GetComponent<AudioSource>();
        audio.clip = clip;
        audio.transform.position = sourceTransform.position;
        audio.Play();
        audioObject.GetComponent<RecycleAudio>().Recycle(audio.clip.length);
    }


}
