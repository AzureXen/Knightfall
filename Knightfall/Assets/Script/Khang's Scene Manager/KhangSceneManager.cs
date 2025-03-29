using UnityEngine;

public class KhangSceneManager : MonoBehaviour
{
    public GameObject sceneTrigger;
    void Start()
    {
        sceneTrigger = GameObject.FindGameObjectWithTag("SceneTrigger");
        if (sceneTrigger != null)
        {
            sceneTrigger.SetActive(false);
        }
        else
        {
            Debug.Log("Cannot find SceneTrigger");
        }
    }
}
