using UnityEngine;
using UnityEngine.SceneManagement;
public class SelectStageMenu : MonoBehaviour
{
    public void backToMainMenu()
    {
        gameObject.SetActive(false);
    }
    public void changeToScene1()
    {
        SceneManager.LoadScene("NhatDang");
    }
    public void changeToScene2()
    {
        SceneManager.LoadScene("VAN-SAMA");
    }
    public void changeToScene3()
    {
        Debug.Log("Scene unavailable");
    }
    public void changeToScene4()
    {
        SceneManager.LoadScene("Khang");
    }
    public void changeToScene5()
    {
        Debug.Log("Scene unavailable");
    }
    public void changeToScene6()
    {
        SceneManager.LoadScene("Game");
    }
}
