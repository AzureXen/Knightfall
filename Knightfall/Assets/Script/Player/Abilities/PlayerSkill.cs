using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerSkill : MonoBehaviour
{
    public Boolean isCasting = false;

    public GameObject holySkillHitbox;

    public GameObject thunderSkillHitbox;
    public GameObject thunderUltHitbox;

    public GameObject flameBeamSkillHitbox;

    void Update()
    {

    }
    public void SkillReg(float skillDuration, string skillName, GameObject skill)
    {
        if (skillName == "holyBlade")
        {
            StartCoroutine(HolyBladeAttack(skillDuration, skill));
        }
        if (skillName == "thunder")
        {
            StartCoroutine(ThunderStrikeAttack(skillDuration, skill));
        }
        if (skillName == "flame")
        {
            StartCoroutine(FlameDevastationAttack(skillDuration, skill));
        }
        if (skillName == "storm")
        {
            StartCoroutine(ThunderStormUlt(skillDuration, skill));
        }
    }
    protected IEnumerator HolyBladeAttack(float skillDuration, GameObject skill)
    {
        Transform hitboxSpawnPos = skill.transform.GetChild(1).transform;
        yield return new WaitForSeconds(1f);

        // Define hitbox spawn positions & sizes
        Vector3[] firstWaveOffsets = { new Vector3(2, 0, 0), new Vector3(-2, 0, 0) };
        Vector3[] secondWaveOffsets = { new Vector3(6, 0, 0), new Vector3(-6, 0, 0) };
        Vector3 hitboxSize = new Vector3(2, 2, 1);

        List<GameObject> firstWave = SpawnHitboxes(firstWaveOffsets, hitboxSpawnPos, hitboxSize);
        yield return new WaitForSeconds(0.5f);

        List<GameObject> secondWave = SpawnHitboxes(secondWaveOffsets, hitboxSpawnPos, new Vector3(3, 3, 1));
        yield return new WaitForSeconds(skillDuration - 0.5f);

        DestroyHitboxes(firstWave);
        yield return new WaitForSeconds(0.5f);
        DestroyHitboxes(secondWave);
    }

    protected IEnumerator ThunderStrikeAttack(float skillDuration, GameObject skill)
    {
        Transform hitboxSpawnPos = skill.transform.GetChild(1).transform;
        float speed = 3f;
        float duration = skillDuration - 2; // How long the hitboxes last before getting destroyed

        // Define 8 directions in a circular pattern
        Vector3[] directions = new Vector3[]
        {
        new Vector3(0, 1, 0),   // Up
        new Vector3(1, 1, 0),   // Up-Right
        new Vector3(1, 0, 0),   // Right
        new Vector3(1, -1, 0),  // Down-Right
        new Vector3(0, -1, 0),  // Down
        new Vector3(-1, -1, 0), // Down-Left
        new Vector3(-1, 0, 0),  // Left
        new Vector3(-1, 1, 0)   // Up-Left
        };

        List<GameObject> thunderHitboxes = new List<GameObject>();

        // Spawn hitboxes in all 8 directions
        foreach (var dir in directions)
        {
            GameObject hitbox = Instantiate(thunderSkillHitbox, hitboxSpawnPos.position, Quaternion.identity);
            Rigidbody2D rb = hitbox.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir.normalized * speed;
            }
            thunderHitboxes.Add(hitbox);
        }

        PlayerMovement playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerMovement.speedBoost = 1.2f;
        yield return new WaitForSeconds(4f);
        playerMovement.speedBoost = 1f;

        yield return new WaitForSeconds(duration -4f);
        playerMovement.speedBoost = 1f;
        DestroyHitboxes(thunderHitboxes);
    }

    protected IEnumerator FlameDevastationAttack(float skillDuration, GameObject skill)
    {
        Transform hitboxSpawnPos = skill.transform.GetChild(1).transform;
        yield return new WaitForSeconds(1f);

        // Define 4 angles for the "X" shape (45-degree increments)
        float[] angles = { 45f, 135f, -45f, -135f };

        // Define position offsets (2,2,0) in respective directions
        Vector3[] offsets =
        {
            new Vector3(3.5f, -3.5f, 0),
            new Vector3(3.5f, 3.5f, 0),
            new Vector3(-3.5f, -3.5f, 0),
            new Vector3(-3.5f, 3.5f, 0)
        };

        List<GameObject> flameHitboxes = new List<GameObject>();

        for (int i = 0; i < angles.Length; i++)
        {
            GameObject hitbox = Instantiate(flameBeamSkillHitbox, hitboxSpawnPos.position + offsets[i], Quaternion.Euler(0, 0, angles[i]), hitboxSpawnPos);

            hitbox.transform.localScale = new Vector3(1, -2.5f, 1);

            flameHitboxes.Add(hitbox);
        }

        yield return new WaitForSeconds(skillDuration-2.5f);

        DestroyHitboxes(flameHitboxes);
    }

    protected IEnumerator ThunderStormUlt(float skillDuration, GameObject skill)
    {
        Transform hitboxSpawnPos = skill.transform.GetChild(0).transform;
        GameObject hitbox = Instantiate(thunderUltHitbox, transform.position, Quaternion.identity, hitboxSpawnPos);
        
        hitbox.transform.localScale = new Vector3(2, 2, 1);
        hitbox.transform.parent = null;

        yield return new WaitForSeconds(skillDuration);

        Destroy(hitbox);
    }

    private List<GameObject> SpawnHitboxes(Vector3[] positions, Transform parent, Vector3 size)
    {
        List<GameObject> hitboxes = new List<GameObject>();
        foreach (var pos in positions)
        {
            GameObject hitbox = Instantiate(holySkillHitbox, transform.position, Quaternion.identity, parent);
            hitbox.transform.position += pos;
            hitbox.transform.localScale = size;
            hitbox.transform.parent = null;
            hitboxes.Add(hitbox);
        }
        return hitboxes;
    }

    private void DestroyHitboxes(List<GameObject> hitboxes)
    {
        if (hitboxes != null)
        {
            foreach (var hitbox in hitboxes)
            {
                Destroy(hitbox);
            }
        }
    }
}
