using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : MonoBehaviour
{
    [SerializeField] float attackRange = 10f;
    [SerializeField] float tooCloseRange = 5f;  // too close, do not fire projectile

    [SerializeField] GameObject projectilePrefab = null;

    [SerializeField] float attackCooldown = 2f;

    GameObject target = null;

    bool readyToFire = true;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (readyToFire)
        {
            Vector3 toTarget = target.transform.position - transform.position;
            toTarget.y = 0;

            if (toTarget.sqrMagnitude > Mathf.Pow(tooCloseRange, 2) && toTarget.sqrMagnitude <= Mathf.Pow(attackRange, 2))
            {
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(toTarget));
                projectile.GetComponent<Projectile>().InitializeProjectile(transform.forward);
                StartCoroutine(StartCooldown(attackCooldown));
            }
        }
    }

    IEnumerator StartCooldown(float duration)
    {
        readyToFire = false;
        yield return new WaitForSeconds(duration);
        readyToFire = true;
    }
}
