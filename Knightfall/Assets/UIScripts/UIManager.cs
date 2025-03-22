using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject pausePanel;
    public GameObject optionPanel;


    private void Start()
    {
    }
    public void OnRestartPress()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnContinuePress()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void OnQuitPress()
    {
        Application.Quit();
    }
    public void OnPausePress()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void OnSettingsPress()
    {
        optionPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void OnBackFromOptionPress()
    {
        pausePanel.SetActive(true);
        optionPanel.SetActive(false);
    }
}
