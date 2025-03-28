using System.Collections;
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
    public void playBattleEndBGM()
    {
        StartCoroutine(battleEndBGM());
    }
    IEnumerator battleEndBGM()
    {
        BGMAudioScript.audioSource.loop = false;
        BGMAudioScript.changeBGM(RoninBGMClips[2], 0.5f);
        yield return new WaitForSeconds(10f);
        BGMAudioScript.changeBGM(RoninBGMClips[0], 1f);
        BGMAudioScript.audioSource.loop = true;
    }
}
