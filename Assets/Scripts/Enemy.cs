using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int health = 5;

    Coroutine hitRoutine = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amt)
    {
        health -= amt;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (hitRoutine != null)
        {
            StopCoroutine(hitRoutine);
        }
        Debug.Log("Dead!");
        GameController.instance.onEnemyDestroyedCallback.Invoke();
        Destroy(this.transform.parent.gameObject);
    }
}
