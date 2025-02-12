using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject pauseUI;
    public void OnRestartPress()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnContinuePress()
    {
        pauseUI.SetActive(false);
    }

    public void OnQuitPress()
    {
        Application.Quit();
    }
    public void OnPausePress()
    {
        pauseUI.SetActive(true);
    }
}
