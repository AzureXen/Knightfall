using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public Boolean isCasting = false;

    public GameObject holySkillHitbox;
    private GameObject holySkillhitboxInstance; private GameObject holySkillhitbox2Instance;
    private GameObject holySkillhitboxInstance2ndWave; private GameObject holySkillhitbox2Instance2ndWave;

    public GameObject thunderSkillHitbox;
    private GameObject thunderSkillhitboxInstance;

    private int skillFrames = 0;

    public GameObject popUpTextPixel;
    private GameObject TextpopUp;
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
    }
    protected IEnumerator HolyBladeAttack(float skillDuration, GameObject skill)
    {
        ShowSkillText("Holy Smite", Color.Lerp(Color.red, Color.yellow, 0.5f));

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
        ShowSkillText("Thunder Strike", Color.yellow);

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

        yield return new WaitForSeconds(duration);

        DestroyHitboxes(thunderHitboxes);
    }

    private void ShowSkillText(string text, Color color)
    {
        GameObject textPopUp = Instantiate(popUpTextPixel, transform.position, Quaternion.identity);
        TextMeshPro damageDisplayMesh = textPopUp.transform.GetChild(0).GetComponent<TextMeshPro>();
        damageDisplayMesh.color = color;
        damageDisplayMesh.text = text;
    }

    private List<GameObject> SpawnHitboxes(Vector3[] positions, Transform parent, Vector3 size)
    {
        List<GameObject> hitboxes = new List<GameObject>();
        foreach (var pos in positions)
        {
            GameObject hitbox = Instantiate(holySkillHitbox, transform.position, Quaternion.identity, parent);
            hitbox.transform.position += pos;
            hitbox.transform.localScale = size;
            //hitbox.transform.parent = null;
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
