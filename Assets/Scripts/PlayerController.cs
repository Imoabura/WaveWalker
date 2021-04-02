using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float runSpeed = 15f;
    [SerializeField] float airSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;

    [Header("Jump Properties")]
    [SerializeField] float gravityModifier = 2.5f;

    [Header("Misc Attributes")]
    [SerializeField] float deadZone = .1f;
    [SerializeField] float runDelay = 1.5f; // seconds until run activates

    [Header("Test Mats, ToBeRemoved")]
    [SerializeField] Material redMat;
    [SerializeField] Material greenMat;
    [SerializeField] Material blueMat;
    [SerializeField] MeshRenderer meshRenderer;

    public bool isGrounded { get { return _isGrounded; } }
    bool _isGrounded = false;

    float timer = 0f;

    Rigidbody rb;
    Camera cam;
    Joystick joystick;
    Button jumpButton;

    float horizontalMove;
    float verticalMove;

    public enum PlayerState
    {
        NORMAL,
        RUNNING,
        JUMPING,
    }

    PlayerState currentState = PlayerState.NORMAL;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        joystick = GameObject.Find("Fixed Joystick").GetComponent<Joystick>();
        joystick.DeadZone = deadZone;
        jumpButton = GameObject.Find("JumpButton").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
        Vector3 camRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;

        horizontalMove = joystick.Horizontal;
        verticalMove = joystick.Vertical;

        Vector3 movement = (camForward * verticalMove + camRight * horizontalMove).normalized;

        switch(currentState)
        {
            default:
            case PlayerState.NORMAL:
                movement *= moveSpeed;
                meshRenderer.material = greenMat;
                break;
            case PlayerState.RUNNING:
                movement *= runSpeed;
                meshRenderer.material = blueMat;
                break;
            case PlayerState.JUMPING:
                movement *= jumpSpeed;
                meshRenderer.material = redMat;
                break;
        }

        rb.velocity = movement + Vector3.up * rb.velocity.y;

        LayerMask terrainMask = LayerMask.GetMask("Terrain");

        if (movement.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(movement.normalized);
        }

        if (Physics.OverlapBox(transform.position - Vector3.up * 1, new Vector3(.5f, .05f, .5f), Quaternion.identity, terrainMask).Length > 0)
        {
            _isGrounded = true;
            if (currentState == PlayerState.JUMPING)
            {
                currentState = PlayerState.NORMAL;
            }
        }
        else
        {
            _isGrounded = false;
            currentState = PlayerState.JUMPING;
        }

        if (currentState == PlayerState.JUMPING && rb.velocity.y <= 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * Time.deltaTime * gravityModifier;
        }

        if (currentState != PlayerState.NORMAL)
        {
            timer = 0f;
        }
        if (movement.sqrMagnitude <= 0)
        {
            if (currentState == PlayerState.RUNNING)
            {
                currentState = PlayerState.NORMAL;
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= runDelay)
            {
                currentState = PlayerState.RUNNING;
                timer = 0f;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position - Vector3.up * 1, new Vector3(.5f, .05f, .5f));
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            rb.velocity += Vector3.up * jumpSpeed;
        }
    }
}
