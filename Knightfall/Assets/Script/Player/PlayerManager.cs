using System.Collections;
using UnityEngine;

public class PlayerManager : EntityManager
{
    Animator am;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        entityMovement = GetComponent<PlayerMovement>();
        am = GetComponent<Animator>();
        EntityName = "Player";
    }

    protected override IEnumerator FlashRed()
    {
        am.SetTrigger("Hurt");
        yield return new WaitForSeconds(0.7f);
    }

    public void StartFlashing()
    {
        if (flashCoroutine == null)
        {
            flashCoroutine = StartCoroutine(base.FlashRed());
        }
    }

    public void FlashGreenTwice()
    {
        StartCoroutine(FlashGreenCoroutine());
    }

    private IEnumerator FlashGreenCoroutine()
    {
        Color flashColor = new Color(0f, 1f, 0f); 
        float flashDuration = 0.1f; 

        for (int i = 0; i < 2; i++) 
        {
            sr.color = flashColor; 
            yield return new WaitForSeconds(flashDuration);
            sr.color = defaultColor; 
            yield return new WaitForSeconds(flashDuration); 
        }
    }


}

/*
 using System.Collections;
using UnityEngine;

public class PlayerManager : EntityManager
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        entityMovement = GetComponent<PlayerMovement>();
        EntityName = "Player";  
        defaultColor = Color.white;
    }

    protected override IEnumerator FlashRed()
    {
        sr.color = new Color(2,2,2);
        yield return null;

        float timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            sr.color = Color.Lerp(sr.color, defaultColor, timer / flashDuration);
            yield return null;
        }

        sr.color = defaultColor;
        flashCoroutine = null;
    }
}
 */