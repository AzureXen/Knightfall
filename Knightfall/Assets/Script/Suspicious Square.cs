using UnityEngine;

public class SuspiciousSquare : MonoBehaviour
{
    private SceneController sc;
    private void Start()
    {
        sc = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            sc.LoadScene(0);
        }
    }
}
