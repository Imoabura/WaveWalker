using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargettingButton : MonoBehaviour
{
    Enemy target = null;
    Skill skill = null;
    int damage = 1;

    Coroutine countdownDestroyRoutine = null;
    Coroutine pressedRoutine = null;

    public void Update()
    {
        if (target != null)
        {
            this.gameObject.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(target.transform.position);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void InitializeUI(Skill usedSkill, Enemy targetEnemy, float lifetime, int dmg)
    {
        skill = usedSkill;
        target = targetEnemy;
        damage = dmg;
        countdownDestroyRoutine = StartCoroutine(DestroyUI(lifetime));
    }

    public void Targetted()
    {
        target.TakeDamage(damage);
        if (pressedRoutine == null)
        {
            pressedRoutine = StartCoroutine(DestroyUI(.3f));
        }
    }

    IEnumerator DestroyUI(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        Destroy(this.gameObject);
    }
}
