using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int dmg = 1;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float lifetime = 3f;

    Rigidbody rb;
    Coroutine selfDestruct = null;

    public void InitializeProjectile(Vector3 moveDir)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = moveSpeed * moveDir.normalized;
        selfDestruct = StartCoroutine(StartSelfDestruct(lifetime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.parent.GetComponent<PlayerCombat>().TakeDamage(1);
            StopCoroutine(selfDestruct);
            Destroy(this.gameObject);
        }
    }

    IEnumerator StartSelfDestruct(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }
}
