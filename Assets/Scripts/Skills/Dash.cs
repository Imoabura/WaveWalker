using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash", menuName = "Skills/Dash")]
public class Dash : Skill
{
    [SerializeField] float dashSpeed = 50f;

    public override void ActivateSkill()
    {
        playerControl.SetMoveDir(playerControl.gameObject.transform.forward);
        playerControl.SetMoveSpeed(dashSpeed);
        playerControl.TogglePlayerUseGravity(false);
        playerControl.TransitionState(PlayerController.PlayerState.LOCKINPUT);
    }

    public override void DeactivateSkill()
    {
        playerControl.TransitionState(PlayerController.PlayerState.NORMAL);
        playerControl.TogglePlayerUseGravity(true);
    }
}
