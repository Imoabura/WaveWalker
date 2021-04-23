using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [SerializeField] protected string _skillName = "";
    [SerializeField] protected float _cooldown = 1f;
    [SerializeField] protected bool _lockMovement = true;
    [SerializeField] protected float _skillDuration = 0f;
    [SerializeField] protected bool _attackReady = true;

    [Header("Time Slow Properties")]
    [SerializeField] protected float _slowDuration = .1f;
    [SerializeField] protected float _slowPercent = .5f;

    public string skillName { get { return _skillName; } }
    public float cooldown { get { return _cooldown; } }
    public bool lockMovement { get { return _lockMovement; } }
    public float skillDuration { get { return _skillDuration; } }
    public float slowDuration { get { return _slowDuration; } }
    public float slowPercent { get { return _slowPercent; } }

    protected PlayerController playerControl = null;

    public bool attackReady { get { return _attackReady; } }

    public void SetReady(bool isReady)
    {
        _attackReady = isReady;
    }

    public void GetPlayerController(PlayerController pc)
    {
        playerControl = pc;
    }

    public abstract void ActivateSkill();

    public abstract void DeactivateSkill();
}
