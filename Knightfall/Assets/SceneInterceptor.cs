using UnityEngine;

public class SceneInterceptor : MonoBehaviour
{
    [SerializeField] private GameObject sceneController;
    private SceneController sceneControllerInstance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneController = GameObject.FindGameObjectWithTag("SceneController");
        sceneControllerInstance = sceneController.GetComponent<SceneController>();
    }

    // Update is called once per frame
    public void OnTriggerEnter2D(Collider2D cool)
    {
        if (cool.CompareTag("Player"))
        {
            sceneControllerInstance.LoadScene("Game");
        }
    }
}
