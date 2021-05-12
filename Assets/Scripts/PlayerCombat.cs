using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Player Properties")]
    [SerializeField] int maxHealth = 10;

    [Header("Other Properties")]
    [SerializeField] float dashDistance = 5f;
    [SerializeField] float timeSlowPercent = .5f;
    [SerializeField] float timeSlowDuration = 1f;
    [SerializeField] float attackCooldown = 1f;

    [Header("Slotted Skills")]  // Button mapping goes from Left to Top (around jump button)
    [SerializeField] Skill skillOne = null;
    [SerializeField] Skill skillTwo = null;

    List<Skill> skills = new List<Skill>();

    PlayerController playerControl = null;

    int currentHealth;

    public delegate void OnDamageTaken(int value);
    public OnDamageTaken onDamageTakenCallback;

    public delegate void OnCooldown(int index, float fillAmount);
    public OnCooldown onCooldownCallback;

    public int totalHealth { get { return maxHealth; } }
    public int health { get { return currentHealth; } }

    private void Start()
    {
        playerControl = GetComponent<PlayerController>();
        skillOne.GetPlayerController(playerControl);
        currentHealth = maxHealth;

        skills.Add(skillOne);
        skills.Add(skillTwo);

        foreach(Skill s in skills)
        {
            s.SetReady(true);
        }
    }
    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * dashDistance, Color.red);
    }

    private void OnDrawGizmos()
    {

    }

    public void UseSkillOffCooldown(int index)
    {
        if (index > skills.Count || index < 0)
        {
            return;
        }
        if (skills[index].attackReady)
        {
            if (index == 0)
            {
                StartCoroutine(UseSkill(skills[index], index, true, true));
            }
            else if (index == 1)
            {
                StartCoroutine(UseSkill(skills[index], index, true, false));
            }
        }
        else
        {
            Debug.Log("On Cooldown!");
        }
    }

    void StartCooldown(Skill skill, int index, float duration)
    {
        //timeSlowDuration + attackCooldown
        StartCoroutine(Cooldown(skill, index, duration));
    }

    IEnumerator UseSkill(Skill skill, int index, bool useScaledTime, bool waitForSlowTime)
    {
        GameController.instance.SlowTime(skill.slowDuration, skill.slowPercent, true);
        if (waitForSlowTime)
        {
            playerControl.ShowIndicator(true);
            yield return new WaitForSecondsRealtime(skill.slowDuration);
            playerControl.ShowIndicator(false);
        }

        skill.ActivateSkill();
        StartCooldown(skill, index, skill.cooldown);
        for (int i = 0; i < skills.Count; ++i)  // set quick cooldown for everything else; ensure no other spells can be used in tandem
        {
            if (i != index)
            {
                StartCooldown(skills[i], i, .25f);
            }
        }
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

    IEnumerator Cooldown(Skill skill, int index, float duration)
    {
        float timer = 0f;
        skill.SetReady(false);
        onCooldownCallback.Invoke(index, 1);
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float fillAmount = timer / duration;
            onCooldownCallback.Invoke(index, 1 - fillAmount);
            yield return null;
        }
        onCooldownCallback.Invoke(index, 0);
        skill.SetReady(true);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        onDamageTakenCallback.Invoke(currentHealth);
    }

    void Die()
    {
        GameController.instance.TransitionState(GameController.GameState.GAME_OVER);
    }
}
