using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Detection Ranges")]
    [SerializeField] float detectionRange = 15f;    // Distance to detect target
    [SerializeField] float stalkDistance = 10f;     // Distance to move around target 
    [SerializeField] float avoidanceRange = 3f;     // Distance to consider allies to avoid
    [SerializeField] float avoidTargetRange = 8f;   // Distance to maintain from target

    [Header("Speeds")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float stalkSpeed = 3f;

    /*[SerializeField]*/ GameObject target = null;

    [Header("Move Direction Weights")]
    [SerializeField, Range(0f, 5f)] float avoidClusteringWeight = 1f;     // Avoid sticking close to allies
    [SerializeField, Range(0f, 5f)] float desiredMoveDirWeight = 1f;      // Based on current EnemyMovementState
    [SerializeField, Range(0f, 5f)] float avoidTargetWeight = 2f;               // Stay away from target

    Rigidbody rb = null;
    Vector3 moveDir = Vector3.zero;
    Vector3 movement = Vector3.zero;

    Vector3 targetDir = Vector3.zero;

    Vector3 awayFromCenter = Vector3.zero;

    EnemyMoveState currentState = EnemyMoveState.WAITING;

    public Vector3 MoveDir { get { return moveDir; } }
    
    public enum EnemyMoveState
    {
        WAITING,    // No movement
        ADVANCING,  // Move toward target
        STALKING,   // Maintain StalkDistance from target
        RETREATING, // Moving away from target
    };

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        Vector3 toTarget = target.transform.position - this.transform.position;
        toTarget.y = 0;
        targetDir = toTarget.normalized;

        if (currentState != EnemyMoveState.WAITING)
        {
            transform.rotation = Quaternion.LookRotation(targetDir);    // look at target
        }

        moveDir = CalculateMoveDirWithAvoidance(avoidanceRange);

        movement = moveDir * ((currentState == EnemyMoveState.ADVANCING) ? moveSpeed : stalkSpeed);

        rb.velocity = movement;

        // Transition States based on dist to target
        if (toTarget.sqrMagnitude <= Mathf.Pow(avoidTargetRange, 2))
        {
            TransitionState(EnemyMoveState.RETREATING);
        }
        else if (toTarget.sqrMagnitude <= Mathf.Pow(stalkDistance, 2))
        {
            TransitionState(EnemyMoveState.STALKING);
        }
        else if (toTarget.sqrMagnitude <= Mathf.Pow(detectionRange, 2))
        {
            TransitionState(EnemyMoveState.ADVANCING);
        }
        else
        {
            TransitionState(EnemyMoveState.WAITING);
        }

        if (moveDir != Vector3.zero)
            Debug.DrawRay(transform.position, moveDir * 3f, Color.green);
        Debug.DrawRay(transform.position, transform.forward * 3f, Color.red);
        if (awayFromCenter != Vector3.zero) 
            Debug.DrawRay(transform.position, awayFromCenter * 3f, Color.cyan);
    }

    // Returns Vector3 averaged from current moveDir and a Vector3 representing a direction away from the average center position of nearby allies (given the radius)
    Vector3 CalculateMoveDirWithAvoidance(float radius)
    {
        Vector3 averageCenterPos = Vector3.zero;
        List<GameObject> nearbyAllies = new List<GameObject>();

        LayerMask enemyMask = LayerMask.GetMask("Enemy");

        foreach(Collider c in Physics.OverlapSphere(transform.position, radius, enemyMask))
        {
            if (c.gameObject.GetComponentInChildren<Enemy>() == this.gameObject.GetComponentInChildren<Enemy>())
            {
                break;
            }
            nearbyAllies.Add(c.gameObject);
            averageCenterPos += c.gameObject.transform.position;
        }

        averageCenterPos /= nearbyAllies.Count;

        Vector3 awayFromCenterDir = transform.position - averageCenterPos;
        awayFromCenterDir.y = 0;
        awayFromCenterDir.Normalize();

        awayFromCenter = awayFromCenterDir;

        if (currentState != EnemyMoveState.RETREATING)
        {
            return ((moveDir * desiredMoveDirWeight + awayFromCenterDir * avoidClusteringWeight) / 2).normalized;
        }
        else
        {
            return ((moveDir * avoidTargetWeight + awayFromCenterDir * avoidClusteringWeight) / 2).normalized;
        }
    }

    void TransitionState(EnemyMoveState newState)
    {
        if (newState == currentState)
        {
            return;
        }
        currentState = newState;

        if (currentState == EnemyMoveState.WAITING)
        {
            moveDir = Vector3.zero;
        }
        else if (currentState == EnemyMoveState.ADVANCING)
        {
            moveDir = targetDir;
        }
        else if (currentState == EnemyMoveState.STALKING)
        {
            float rand = (Random.value - .5f) * 2;
            moveDir = (transform.right * rand).normalized;
        }
        else if (currentState == EnemyMoveState.RETREATING)
        {
            moveDir = -targetDir;
        }
    }
}
