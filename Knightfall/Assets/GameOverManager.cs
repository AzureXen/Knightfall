using System.Collections;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject player ;
    public GameObject gameOver;
    void Start()
    {
        StartCoroutine(FindPlayer());

    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            gameOver.SetActive(true);
        }
    }

    private IEnumerator FindPlayer()
    {
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }
    }
}
