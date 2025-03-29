using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerSkillManager : MonoBehaviour
{
    private PlayerSkill skillActive;

    public GameObject holySwordSkill;
    public GameObject holySwordVisual;
    public GameObject holySwordVisualVFX;

    public GameObject thunderSkill;

    public float holySwordSkillCooldown = 10f;
    public float holySwordSkillCooldownTimer = 0;
    public float holySwordSkillDuration = 8;
    public int holySwordDamage;
    public float holySwordKnockbackForce;
    public float holySwordKnockbackDuration;

    public float thunderSkillCooldown = 5f;
    public float thunderSkillCooldownTimer = 0;
    public float thunderSkillDuration = 8;
    public int thunderDamage;
    public float thunderKnockbackForce;
    public float thunderKnockbackDuration;

    public Boolean canCast = true;

    public GameObject popUpTextPixel;
    private GameObject TextpopUp;
    void Start()
    {
        skillActive = GetComponent<PlayerSkill>();
        holySwordSkill = transform.GetChild(0).gameObject;
        //thunder = transform.GetChild(1).gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        if (canCast)
        {
            SpellCast();
        }
        if (holySwordSkillCooldownTimer > 0)
        {
            holySwordSkillCooldownTimer -= Time.deltaTime;
            holySwordSkillCooldownTimer = Mathf.Clamp(holySwordSkillCooldownTimer, 0, holySwordSkillCooldown);
        }

        if (thunderSkillCooldownTimer > 0)
        {
            thunderSkillCooldownTimer -= Time.deltaTime;
            thunderSkillCooldownTimer = Mathf.Clamp(thunderSkillCooldownTimer, 0, thunderSkillCooldown);
        }
    }

    void SpellCast()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && holySwordSkillCooldownTimer <= 0)
        {
            string skillName = "holyBlade";
            holySwordSkill.SetActive(true);
            skillActive.SkillReg(holySwordSkillDuration, skillName, holySwordSkill);
            StartCoroutine(SpellCastChosen(holySwordSkill, 4f));
            holySwordSkillCooldownTimer = holySwordSkillCooldown;

            holySwordVisual.GetComponent<Animator>().SetTrigger("activate");
            holySwordVisualVFX.GetComponent<Animator>().SetTrigger("Cast");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && holySwordSkillCooldownTimer > 0)
        {
            TextpopUp = Instantiate(popUpTextPixel, transform.position, Quaternion.identity) as GameObject;
            TextMeshPro damageDisplayMesh = TextpopUp.transform.GetChild(0).GetComponent<TextMeshPro>();
            damageDisplayMesh.color = Color.white; ;
            damageDisplayMesh.text = "On Cooldown";
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && thunderSkillCooldownTimer <= 0)
        {
            string skillName = "thunder";
            thunderSkill.SetActive(true);
            skillActive.SkillReg(thunderSkillDuration, skillName, thunderSkill);
            StartCoroutine(SpellCastChosen(thunderSkill, 4f));
            thunderSkillCooldownTimer = thunderSkillCooldown;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && thunderSkillCooldownTimer > 0)
        {
            TextpopUp = Instantiate(popUpTextPixel, transform.position, Quaternion.identity) as GameObject;
            TextMeshPro damageDisplayMesh = TextpopUp.transform.GetChild(0).GetComponent<TextMeshPro>();
            damageDisplayMesh.color = Color.white; ;
            damageDisplayMesh.text = "On Cooldown";
        }
    }

    private IEnumerator SpellCastChosen(GameObject skill, float duration)
    {
        canCast = false;
        yield return new WaitForSeconds(0.1f);
        canCast = true;

        yield return new WaitForSeconds(duration); //prevent Skill spam

        skill.SetActive(false);
    }
}
