using UnityEngine;
using UnityEngine.UI;

public class RoninCanvasScript : MonoBehaviour
{
    public float healthPercentage = 100f;
    public float motivationPercentage = 100f;
    public Image healthBar;
    public Image motivationBar;
    void Start()
    {
        hideCanvas();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = Mathf.Clamp(healthPercentage, 0, 1);
        motivationBar.fillAmount = Mathf.Clamp(motivationPercentage, 0, 1);
    }
    
    public void showCanvas()
    {
        transform.gameObject.SetActive(true);
    }
    public void hideCanvas()
    {
        transform.gameObject.SetActive(false);
    }
}
