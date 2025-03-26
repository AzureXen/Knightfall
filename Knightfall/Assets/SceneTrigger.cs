using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    private SceneController controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("SceneController").GetComponent<SceneController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            controller.LoadScene(2);
        }
    }
}
