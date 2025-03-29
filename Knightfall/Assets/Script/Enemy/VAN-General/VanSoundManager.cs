using UnityEngine;


public enum SoundType
{
    LEVELUP,
    SKILLCHOSEN,
    PICKUP1,
    PICKUP2,
    PICKUP3,
    ANGELVOICELINE1,
    ANGELVOICELINE2,
    ANGELVOICELINE3,
    SAGEVOICELINE1,
    SAGEVOICELINE2,
    SAGEVOICELINE3,
    MONKVOICELINE1,
    MONKVOICELINE2,
    MONKVOICELINE3,
    
    OPENINGBGM,
    WAVEBATTLEBGM,
    BOSSBGM,

    UNDEADHURT,
    UDEADDEAD,

    BOSSSPAWNS,
    BOSSSLICE,
    BOSSEXPLODE
}

[RequireComponent(typeof(AudioSource))]
public class VanSoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    private static VanSoundManager instance;
    private AudioSource audioSource;
    private AudioSource bgmSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }

    public static void PlayBGM(SoundType bgm, float volume = 1)
    {
        if (instance == null) return;

        // Stop current BGM before playing new one
        instance.bgmSource.clip = instance.soundList[(int)bgm];
        instance.bgmSource.volume = Mathf.Clamp(volume, 0f, 1f);
        instance.bgmSource.Play();
    }

    public static void StopBGM()
    {
        if (instance == null) return;
        instance.bgmSource.Stop();
    }
}