using System.Collections;
using UnityEngine;

public class BGMScript : MonoBehaviour
{
    public AudioSource audioSource;
    [SerializeField] private Coroutine changeAudioCoroutine;
    [SerializeField] private float audioChangeDelay;
    [SerializeField] private float audioChangeDelayTimer;
    [SerializeField] private AudioClip currentClip;

    [Range(0f, 1f)]
    public float BGMVolume;

    private void Start()
    {
        audioSource.loop = true;
    }

    // Differences between BGMVolume and volume in changeBGM
    // The BGMVolume is the volume that can be changed by our user.
    // and the volume in the changeBGM can be changed by developers.
    // Why? because some audio clips are much louder than the other. And with this, our dev can change how loud it is.
    public void changeBGM(AudioClip clip, float volume)
    {
        if (currentClip == clip) return;

        if(volume > 1 || volume < 0 )
        {
            Debug.LogError("Volume must be in range of {0;1} !");
            Debug.LogError("Clip tried to play: " + clip.name);
            return;
        }

        if(changeAudioCoroutine != null)
        {
            StopCoroutine(changeAudioCoroutine);
        }
        changeAudioCoroutine = StartCoroutine(change(clip, volume));
    }

     
    private IEnumerator change(AudioClip clip, float volume)
    {
        // Stop playing the current clip
        audioSource.Stop();

        audioSource.volume = volume * BGMVolume;
        // Delays for a while
        audioChangeDelayTimer = audioChangeDelay;
        while(audioChangeDelayTimer > 0)
        {
            audioChangeDelayTimer -= Time.deltaTime;
            audioChangeDelayTimer = Mathf.Clamp(audioChangeDelayTimer, 0f, audioChangeDelay);
            yield return null;
        }
        // Change audio
        audioSource.clip = clip;
        audioSource.Play();

        // update current clip
        currentClip = clip;

        changeAudioCoroutine = null;
    }
}
