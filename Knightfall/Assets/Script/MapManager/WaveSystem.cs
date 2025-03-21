using System;
using UnityEngine;

public class WaveSystem : MonoBehaviour {
    [SerializeField] private WaveSystemTrigger trigger;

    public Transform spawnPos;
    private Transform playerPos;

    public GameObject Skeleton;
    public GameObject Necromancer;

    private GameObject spawnInstance;

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
        spawnInstance = Instantiate(Skeleton, transform.position, Quaternion.identity, spawnPos);
        spawnInstance.transform.position += new Vector3(20, 20, 1);

        SkeletonActions skeletonActions = spawnInstance.GetComponent<SkeletonActions>();
        SkeletonMovement skeletonMovement = spawnInstance.GetComponent<SkeletonMovement>();

        skeletonActions.isSpotted = true;
        skeletonMovement.backOffSpeed = 0.1f;

        spawnInstance.transform.parent = null;

        wave++;
    }

    private void StartBattle2()
    {
        spawnInstance = Instantiate(Necromancer, transform.position, Quaternion.identity, spawnPos);
        spawnInstance.transform.position += new Vector3(0, 20, 1);

        NecromancerActions necromancerActions = spawnInstance.GetComponent<NecromancerActions>();
        NecromancerMovement necromancerMovement = spawnInstance.GetComponent<NecromancerMovement>();

        necromancerActions.isSpotted = true;
        necromancerMovement.backOffSpeed = 0.1f;

        spawnInstance.transform.parent = null;

        wave++;
    }

    private void StartBattle3()
    {
        spawnInstance = Instantiate(Necromancer, transform.position, Quaternion.identity, spawnPos);
        spawnInstance.transform.position = new Vector3(0, -20, 1);

        //NecromancerActions necromancerActions = spawnInstance.GetComponent<NecromancerActions>();
        NecromancerMovement necromancerMovement = spawnInstance.GetComponent<NecromancerMovement>();

        //necromancerActions.isSpotted = true;
        necromancerMovement.backOffSpeed = 0.1f;

        spawnInstance.transform.parent = null;

        wave=0;
    }
}
