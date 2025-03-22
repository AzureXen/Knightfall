using UnityEngine;

public class RoninBGM : MonoBehaviour
{
    private BGMScript BGMAudioScript;
    public AudioClip[] RoninBGMClips;
    private void Start()
    {
        BGMAudioScript = GameObject.FindGameObjectWithTag("BGM").GetComponent<BGMScript>();
        playPreBattleBGM();
    }

    public void playPreBattleBGM()
    {
        BGMAudioScript.changeBGM(RoninBGMClips[0], 1f);
    }

    public void playBattleBGM()
    {
        BGMAudioScript.changeBGM(RoninBGMClips[1], 0.5f);
    }
}
