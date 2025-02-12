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
