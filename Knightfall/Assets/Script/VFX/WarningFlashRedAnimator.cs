using System.Collections;
using UnityEngine;

public class WarningFlashRedAnimator : MonoBehaviour
{
    public const string FlashRed = "WarningFlashRed";
    public const string FlashRedNone = "WarningFlashRedNone";
    private Animator am;
    private string currentState;

    private Coroutine flashRedCoroutine;
    private void Start()
    {
        am = GetComponent<Animator>();
    }
    private void PlayAnimation(string newState)
    {
        if (currentState == newState) return;
        am.Play(newState);
        currentState = newState;
    }

    public void PlayWarningFlashRed()
    {
        if (flashRedCoroutine != null) StopCoroutine(flashRedCoroutine);

        flashRedCoroutine = StartCoroutine(FlashRedCoroutine());
    }

    private IEnumerator FlashRedCoroutine()
    {
        PlayAnimation(FlashRed);
        yield return new WaitUntil(() => this.IsAnimationFinished(FlashRed));
        PlayAnimation(FlashRedNone);
        flashRedCoroutine = null;
    }

    public bool IsAnimationFinished(string animationName)
    {
        AnimatorStateInfo stateInfo = am.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1f;
    }
}
