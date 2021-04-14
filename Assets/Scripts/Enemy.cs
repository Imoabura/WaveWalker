using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int health = 5;
    [SerializeField] int dmg = 1;

    [Header("TestMats")]
    [SerializeField] Material regularMat;
    [SerializeField] Material hitMat;

    MeshRenderer meshRenderer;

    Coroutine hitRoutine = null;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
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
        else
        {
            hitRoutine = StartCoroutine(FlashDamage());
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
        Destroy(this.gameObject);
    }

    IEnumerator FlashDamage()
    {
        for (int i = 0; i < 4; ++i)
        {
            meshRenderer.material = (i % 2 == 0) ? hitMat : regularMat;
            yield return new WaitForSeconds(.15f);
        }
        meshRenderer.material = regularMat;
    }
}
