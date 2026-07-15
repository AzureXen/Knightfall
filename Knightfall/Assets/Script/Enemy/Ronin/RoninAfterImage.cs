using UnityEngine;

public class RoninAfterImage : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;
    private float fadeTimer;

    [SerializeField] private float lifetime = 0.4f; 
    [SerializeField] private Color ghostColor = new Color(1f, 1f, 1f, 0.6f); 

    public void InitGhost(Sprite targetSprite, bool flipX, Vector3 scale)
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = targetSprite;
        sr.flipX = flipX;
        transform.localScale = scale;

        sr.color = ghostColor;
        originalColor = ghostColor;
        fadeTimer = lifetime;
    }

    private void Update()
    {
        fadeTimer -= Time.deltaTime;

        if (fadeTimer <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            float alpha = Mathf.Lerp(0f, originalColor.a, fadeTimer / lifetime);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }
    }
}
