using System.Collections;
using UnityEngine;

public class EvilWizardHealth : Health
{
    public GameObject changeSceneArea;
    private void Start()
    {
        KhangSceneManager sceneManager = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<KhangSceneManager>();
        changeSceneArea = sceneManager.sceneTrigger.gameObject;
    }
    public override void Update()
    {
        if (health <= 0)
        {
            StartCoroutine(onDeath());
        }
    }
    IEnumerator onDeath()
    {
        changeSceneArea.SetActive(true);
        yield return null;
        Destroy(gameObject);
    }
}
