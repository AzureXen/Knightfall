using System;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleSystemTrigger trigger;

    private Transform playerPos;
    public Transform spawnTransform;

    public GameObject Skeleton;
    public GameObject Necromancer;

    private GameObject spawnInstance1; 
    private GameObject spawnInstance2;

    private GameObject spawnInstance3;

    private int stages = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trigger.OnPlayerEnterTrigger += ColliderTrigger_OnPlayerEnterTrigger;
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void ColliderTrigger_OnPlayerEnterTrigger(object senderm, System.EventArgs e)
    {
        if (stages == 0 && spawnInstance1 == null && spawnInstance2 == null) { StartBattle(); stages++; }
        if (stages == 1) {
            if (spawnInstance1 != null && spawnInstance2 != null) {
                SkeletonHeadActions skeleton1Actions = spawnInstance1.GetComponent<SkeletonHeadActions>(); SkeletonHeadActions skeleton2Actions = spawnInstance2.GetComponent<SkeletonHeadActions>();
                if (skeleton1Actions.isSpotted && skeleton2Actions.isSpotted) { spawnInstance1.transform.position = playerPos.position + new Vector3(0, -10, 0); spawnInstance2.transform.position = playerPos.position + new Vector3(0, -10, 0); StartBattle2(); stages++; }
            }
            else if (spawnInstance1 != null && spawnInstance2 == null) { SkeletonHeadActions skeleton1Actions = spawnInstance1.GetComponent<SkeletonHeadActions>(); if (skeleton1Actions.isSpotted) StartBattle2(); stages++; }
            else if (spawnInstance2 != null && spawnInstance1 == null) { SkeletonHeadActions skeleton2Actions = spawnInstance2.GetComponent<SkeletonHeadActions>(); if (skeleton2Actions.isSpotted) StartBattle2(); stages++; }

            else if (spawnInstance3 == null && spawnInstance1 == null && spawnInstance2 == null) { StartBattle2(); stages++; } 
        }
    }

    private void StartBattle()
    {
        spawnInstance1 = Instantiate(Skeleton, transform.position, Quaternion.identity, spawnTransform);
        spawnInstance1.transform.position += new Vector3(-3, 5, 0);
        spawnInstance1.transform.parent = null;

        spawnInstance2 = Instantiate(Skeleton, transform.position, Quaternion.identity, spawnTransform);
        spawnInstance2.transform.position += new Vector3(0, 4, 0);
        spawnInstance2.transform.parent = null;

        transform.localPosition = new Vector3(20, 5, 0);
    }

    private void StartBattle2()
    {
        spawnInstance3 = Instantiate(Necromancer, transform.position, Quaternion.identity, spawnTransform);
        spawnInstance3.transform.position += new Vector3(1, -2, 0);

        spawnInstance3.transform.parent = null;
    }
}
