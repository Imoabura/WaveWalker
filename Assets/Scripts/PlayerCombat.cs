using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float atkRange = .5f;
    [SerializeField] float dashDistance = 5f;
    [SerializeField] float timeSlowPercent = .5f;
    [SerializeField] float timeSlowDuration = 1f;
    [SerializeField] float attackCooldown = 1f;

    [Header("Slotted Skills")]  // Button mapping goes from Left to Top (around jump button)
    [SerializeField] Skill skillOne = null;
    [SerializeField] Skill skillTwo = null;

    Coroutine timeSlowCoroutine = null;
    PlayerController playerControl = null;

    bool attackReady = true;

    private void Start()
    {
        playerControl = GetComponent<PlayerController>();
        skillOne.loadPlayer(playerControl);
    }

    public void UseSkillOffCooldown(int index)
    {
        if (attackReady)
        {
            switch (index)
            {
                default:
                case 0:
                    StartCoroutine(UseSkill(skillOne, true));
                    break;
                case 1:
                    // skill 2
                    break;
            }
        }
        else
        {
            Debug.Log("On Cooldown!");
        }
    }

    void StartCooldown(float duration)
    {
        StartCoroutine(Cooldown(timeSlowDuration + attackCooldown));
    }

    IEnumerator UseSkill(Skill skill, bool useScaledTime)
    {
        skill.ActivateSkill();
        StartCooldown(skill.cooldown);
        if (useScaledTime)
        {
            yield return new WaitForSeconds(skill.skillDuration);
        }
        else
        {
            yield return new WaitForSecondsRealtime(skill.skillDuration);
        }
        skill.DeactivateSkill();
    }

    IEnumerator Cooldown(float duration)
    {
        attackReady = false;
        yield return new WaitForSecondsRealtime(duration);
        attackReady = true;
    }

    IEnumerator SlowDownTime(float duration)
    {
        Time.timeScale = timeSlowPercent;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * dashDistance, Color.red);
    }

    private void OnDrawGizmos()
    {
        
    }
}
