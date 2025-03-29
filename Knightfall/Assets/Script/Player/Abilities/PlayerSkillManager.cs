using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillManager : MonoBehaviour
{
    private Dictionary<string, bool> unlockedSkills = new Dictionary<string, bool>();
    private PlayerSkill skillActive;

    public GameObject holySwordSkill;
    public GameObject holySwordVisual;
    public GameObject holySwordVisualVFX;

    public GameObject thunderSkill;

    public GameObject flameSkill;

    public GameObject holySwordUlt;

    public GameObject thunderUlt;

    public GameObject flameUlt;

    public GameObject holySwordSkillIcon;
    public GameObject thunderSkillIcon;
    public GameObject flameSkillIcon;
    public GameObject holySwordUltIcon;
    public GameObject thunderUltIcon;
    public GameObject flameUltIcon;

    public float holySwordSkillCooldown = 10f;
    public float holySwordSkillCooldownTimer = 0;
    public float holySwordSkillDuration = 8;

    public float thunderSkillCooldown = 5f;
    public float thunderSkillCooldownTimer = 0;
    public float thunderSkillDuration = 8;

    public float flameSkillCooldown = 10f;
    public float flameSkillCooldownTimer = 0;
    public float flameSkillDuration = 8;

    public float holySwordUltCooldown = 20f;
    public float holySwordUltCooldownTimer = 0;
    public float holySwordUltDuration = 8;

    public float thunderUltCooldown = 20f;
    public float thunderUltCooldownTimer = 0;
    public float thunderUltDuration = 4.8f;

    public float flameUltCooldown = 5f;
    public float flameUltCooldownTimer = 0;
    public float flameUltDuration = 2.4f;

    public Boolean canCast = true;
    public Boolean skillDisable = false;

    void Start()
    {
        skillActive = GetComponent<PlayerSkill>();
        holySwordSkill = transform.GetChild(1).gameObject;
        VanSoundManager.PlayBGM(SoundType.OPENINGBGM, 0.4f);
        //thunder = transform.GetChild(1).gameObject;

        unlockedSkills["HolySwordSkill"] = false;
        unlockedSkills["ThunderSkill"] = false;
        unlockedSkills["FlameSkill"] = false;
        unlockedSkills["HolySwordUlt"] = false;
        unlockedSkills["ThunderUlt"] = false;
        unlockedSkills["FlameUlt"] = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canCast && !skillDisable)
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

        if (flameSkillCooldownTimer > 0)
        {
            flameSkillCooldownTimer -= Time.deltaTime;
            flameSkillCooldownTimer = Mathf.Clamp(flameSkillCooldownTimer, 0, flameSkillCooldown);
        }

        if (holySwordUltCooldownTimer > 0)
        {
            holySwordUltCooldownTimer -= Time.deltaTime;
            holySwordUltCooldownTimer = Mathf.Clamp(holySwordUltCooldownTimer, 0, holySwordUltCooldown);
        }

        if (thunderUltCooldownTimer > 0)
        {
            thunderUltCooldownTimer -= Time.deltaTime;
            thunderUltCooldownTimer = Mathf.Clamp(thunderUltCooldownTimer, 0, thunderUltCooldown);
        }

        if (flameUltCooldownTimer > 0)
        {
            flameUltCooldownTimer -= Time.deltaTime;
            flameUltCooldownTimer = Mathf.Clamp(flameUltCooldownTimer, 0, flameUltCooldown);
        }

        UpdateSkillCooldownUI(holySwordSkillCooldownTimer, holySwordSkillCooldown, holySwordSkillIcon);
        UpdateSkillCooldownUI(thunderSkillCooldownTimer, thunderSkillCooldown, thunderSkillIcon);
        UpdateSkillCooldownUI(flameSkillCooldownTimer, flameSkillCooldown, flameSkillIcon);
        UpdateSkillCooldownUI(holySwordUltCooldownTimer, holySwordUltCooldown, holySwordUltIcon);
        UpdateSkillCooldownUI(thunderUltCooldownTimer, thunderUltCooldown, thunderUltIcon);
        UpdateSkillCooldownUI(flameUltCooldownTimer, flameUltCooldown, flameUltIcon);
    }

    void SpellCast()
    {
        if (unlockedSkills["HolySwordSkill"] == true && holySwordSkillCooldownTimer <= 0)
        {
            string skillName = "holyBlade";
            holySwordSkill.SetActive(true); holySwordSkillIcon.SetActive(true);
            skillActive.SkillReg(holySwordSkillDuration, skillName, holySwordSkill);
            StartCoroutine(SpellCastChosen(holySwordSkill, 2f));
            holySwordSkillCooldownTimer = holySwordSkillCooldown;

            holySwordVisual.GetComponent<Animator>().SetTrigger("activate");
            holySwordVisualVFX.GetComponent<Animator>().SetTrigger("Cast");
        }

        if (unlockedSkills["ThunderSkill"] == true && thunderSkillCooldownTimer <= 0)
        {
            string skillName = "thunder";
            thunderSkill.SetActive(true); thunderSkillIcon.SetActive(true);
            skillActive.SkillReg(thunderSkillDuration, skillName, thunderSkill);
            StartCoroutine(SpellCastChosen(thunderSkill, 4));
            thunderSkillCooldownTimer = thunderSkillCooldown;
        }

        if (unlockedSkills["FlameSkill"] == true && flameSkillCooldownTimer <= 0)
        {
            string skillName = "flame"; flameSkillIcon.SetActive(true);
            flameSkill.SetActive(true); flameSkillIcon.SetActive(true);
            skillActive.SkillReg(flameSkillDuration, skillName, flameSkill);
            StartCoroutine(SpellCastChosen(flameSkill, flameSkillDuration));
            flameSkillCooldownTimer = flameSkillCooldown;
        }

        if (unlockedSkills["HolySwordUlt"] == true && holySwordUltCooldownTimer <= 0 )
        {
            holySwordUlt.SetActive(true); holySwordUltIcon.SetActive(true);
            StartCoroutine(SpellCastChosen(holySwordUlt, holySwordUltDuration));
            holySwordUltCooldownTimer = holySwordUltCooldown;
        }

        if (unlockedSkills["ThunderUlt"] == true && thunderUltCooldownTimer <= 0)
        {
            string skillName = "storm";
            thunderUlt.SetActive(true); thunderUltIcon.SetActive(true);
            skillActive.SkillReg(thunderUltDuration, skillName, thunderUlt);
            StartCoroutine(SpellCastChosen(thunderUlt, 2f));
            thunderUltCooldownTimer = thunderUltCooldown;
        }

        if (unlockedSkills["FlameUlt"] == true && flameUltCooldownTimer <= 0)
        {
            flameUlt.SetActive(true); flameUltIcon.SetActive(true);
            StartCoroutine(SpellCastChosen(flameUlt, flameUltDuration));
            flameUltCooldownTimer = flameUltCooldown;
        }
    }
    public bool UnlockSkill(string skillName)
    {
        if (unlockedSkills.ContainsKey(skillName))
        {
            unlockedSkills[skillName] = true;
            Debug.Log("Unlocked: " + skillName);
            return true;
        }
        return false;
    }

    private IEnumerator SpellCastChosen(GameObject skill, float duration)
    {
        canCast = false;
        yield return new WaitForSeconds(0.1f);
        canCast = true;

        yield return new WaitForSeconds(duration); //prevent Skill spam

        skill.SetActive(false);
    }

    void UpdateSkillCooldownUI(float cooldownTimer, float cooldownDuration, GameObject skillIcon)
    {
        if (skillIcon.activeSelf)
        {
            Image cooldownImage = skillIcon.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            cooldownImage.fillAmount = cooldownTimer / cooldownDuration;
        }
    }

    void DisableSkillCast()
    {
        skillDisable = true;
    }
}
