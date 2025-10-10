using System.Collections;
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
    [SerializeField] AudioClip spotted;
    [SerializeField] AudioClip goblinDeath;
    [SerializeField] AudioClip music;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        GameEventsBus.Instance.Subscribe<PlayerSlash>((l) => { PlaySFX(slash, PlayerComponent.playerTransform); });
        GameEventsBus.Instance.Subscribe<PlayerDamaged>((l) => { PlaySFX(hurt, PlayerComponent.playerTransform); });
        GameEventsBus.Instance.Subscribe<MoneyGained>((l) => { PlaySFX(money, PlayerComponent.playerTransform); });
        GameEventsBus.Instance.Subscribe<GoblinSurprise>((l) => { PlaySFX(spotted, PlayerComponent.playerTransform); });
        GameEventsBus.Instance.Subscribe<GoblinDeath>((l) => { PlaySFX(goblinDeath, PlayerComponent.playerTransform); });
        GameEventsBus.Instance.Subscribe<ReplayMusic>((l) => { StartCoroutine(PlayMusic()); });
    }

    private void Start()
    {
        GameEventsBus.Instance.Publish(new ReplayMusic());
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

    IEnumerator PlayMusic()
    {
        PlaySFX(music, PlayerComponent.playerTransform);
        yield return new WaitForSeconds(music.length);
        GameEventsBus.Instance.Publish(new ReplayMusic());
    }
}
