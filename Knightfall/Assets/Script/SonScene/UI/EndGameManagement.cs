using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManagement : MonoBehaviour
{
    public GameObject gameOverUI; // Assign your Canvas in Inspector
    public GameObject nextLevelUI;

    public GameObject player;

    private bool gameEnded = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverUI.SetActive(false); 
        nextLevelUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        if (gameEnded) return;
        gameEnded = true;
        gameOverUI.SetActive(true); 
    }

    public void WinLevel()
    {
        if (gameEnded) return;
        gameEnded = true;
        nextLevelUI.SetActive(true); 
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Load next scene
    }
}
