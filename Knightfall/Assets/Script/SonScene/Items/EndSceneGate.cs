using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneGate : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "NhatDang";
    [SerializeField] private LetterCollectorCustom letterCollector;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && letterCollector != null)
        {
            if (letterCollector.HasCollectedAllLetters()) // Check if enough letters are collected
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.Log("Not enough letters collected!");
            }
        }
    }
}
