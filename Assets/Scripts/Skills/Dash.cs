using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash", menuName = "Skills/Dash")]
public class Dash : Skill
{
    [SerializeField] float dashSpeed = 50f;
    [SerializeField] bool hitBoxOn = false;
    [SerializeField] int dmg = 1;

    LayerMask dashLayer;
    LayerMask playerLayer;
    Vector3 startPos;

    private void OnEnable()
    {
        dashLayer = LayerMask.NameToLayer("Dash");
        playerLayer = LayerMask.NameToLayer("Player");
    }

    public override void ActivateSkill()
    {
        startPos = playerControl.transform.position;
        playerControl.SetMoveDir(playerControl.gameObject.transform.forward);
        playerControl.SetMoveSpeed(dashSpeed);
        playerControl.TogglePlayerUseGravity(false);
        playerControl.TransitionState(PlayerController.PlayerState.LOCKINPUT);
        playerControl.SetPhysicsLayer(dashLayer);
        if (hitBoxOn)
        {
            DetectAndHitEnemies();
        }
    }

    public override void DeactivateSkill()
    {
        playerControl.TransitionState(PlayerController.PlayerState.NORMAL);
        playerControl.TogglePlayerUseGravity(true);
        playerControl.SetPhysicsLayer(playerLayer);
    }

    public void DetectAndHitEnemies()
    {
        LayerMask enemyMask = LayerMask.GetMask("Enemy");

        Collider[] hits = Physics.OverlapBox(startPos + playerControl.transform.forward * (.1f * 50 / 2), new Vector3(.75f, .75f, (.1f * 50 + 1) / 2), Quaternion.LookRotation(playerControl.transform.forward), enemyMask);

        foreach(Collider c in hits)
        {
            c.gameObject.GetComponent<Enemy>().TakeDamage(dmg);
        }
    }
}
