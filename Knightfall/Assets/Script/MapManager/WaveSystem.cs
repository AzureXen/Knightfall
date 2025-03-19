using System;
using UnityEngine;

public class WaveSystem : MonoBehaviour {
    [SerializeField] private WaveSystemTrigger trigger;

    public Transform spawnPos;
    private Transform playerPos;

    public GameObject Skeleton;
    public GameObject Necromancer;

    private GameObject spawnInstance3;

    private Coroutine waveCoroutine;

    private int wave = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trigger.OnPlayerEnterTrigger += ColliderTrigger_OnPlayerEnterTrigger;

        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void ColliderTrigger_OnPlayerEnterTrigger(object senderm, System.EventArgs e)
    {
        if (wave == 0) { StartBattle(); }
        if (wave == 1) { StartBattle2(); }
        if (wave == 2) { StartBattle3(); }
    }

    private void StartBattle()
    {
        spawnInstance3 = Instantiate(Skeleton, transform.position, Quaternion.identity, spawnPos);
        spawnInstance3.transform.localScale = new Vector3(40, 20, 1);

        spawnInstance3.transform.parent = null;

        wave++;
    }

    private void StartBattle2()
    {
        spawnInstance3 = Instantiate(Necromancer, transform.position, Quaternion.identity, spawnPos);
        spawnInstance3.transform.localScale = new Vector3(0, 20, 1);

        spawnInstance3.transform.parent = null;

        wave++;
    }

    private void StartBattle3()
    {
        spawnInstance3 = Instantiate(Necromancer, transform.position, Quaternion.identity, spawnPos);
        spawnInstance3.transform.localScale = new Vector3(0, -20, 1);

        spawnInstance3.transform.parent = null;

        wave++;
    }
}
