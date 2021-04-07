using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [SerializeField] protected string _skillName = "";
    [SerializeField] protected float _cooldown = 1f;
    [SerializeField] protected bool _lockMovement = true;
    [SerializeField] protected float _skillDuration = 0f;

    public string skillName { get { return _skillName; } }
    public float cooldown { get { return _cooldown; } }
    public bool lockMovement { get { return _lockMovement; } }
    public float skillDuration { get { return _skillDuration; } }

    protected PlayerController playerControl = null;

    public void loadPlayer(PlayerController pc)
    {
        playerControl = pc;
    }

    public abstract void ActivateSkill();

    public abstract void DeactivateSkill();
}
