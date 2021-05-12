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
    [SerializeField] float jumpSpeed = 5f;

    [Header("Jump Properties")]
    [SerializeField] float gravityMultiplier = 2.5f;

    [Header("Misc Attributes")]
    [SerializeField] float deadZone = .1f;
    [SerializeField] float runDelay = 1.5f; // seconds until run activates

    [Header("Other")]
    [SerializeField] GameObject dashIndicator = null;

    public bool isGrounded { get { return _isGrounded; } }
    bool _isGrounded = false;

    float timer = 0f;

    Rigidbody rb;
    Camera cam;
    Joystick joystick;
    Button jumpButton;

    float horizontalMove;
    float verticalMove;
    float movementMultiplier;
    float gravityModifier;

    Vector3 moveDir;
    Vector3 movement;

    Vector3 camForward;
    Vector3 camRight;

    public enum PlayerState
    {
        NORMAL,
        RUNNING,
        JUMPING,
        LOCKINPUT,   // freeze of player movement inputs (jump, movement), rotation is still okay
        FREEZE,         // full freeze of player inputs (jump, movement, skill activations)
    }

    PlayerState currentState = PlayerState.NORMAL;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;

        joystick = GameObject.Find("Fixed Joystick").GetComponent<Joystick>();
        jumpButton = GameObject.Find("JumpButton").GetComponent<Button>();

        joystick.DeadZone = deadZone;

        movementMultiplier = moveSpeed;
        gravityModifier = gravityMultiplier;

        moveDir = Vector3.zero;
        movement = Vector3.zero;

        camForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
        camRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;

        dashIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = joystick.Horizontal;
        verticalMove = joystick.Vertical;

        if (currentState != PlayerState.LOCKINPUT)
        {
            moveDir = (camForward * verticalMove + camRight * horizontalMove).normalized;
        }

        if (currentState != PlayerState.FREEZE)
        {
            movement = moveDir * movementMultiplier;
        }
        else
        {
            movement = Vector3.zero;
        }

        if (currentState != PlayerState.FREEZE && movement.sqrMagnitude > 0)  // player faces direction of movement if moving
        {
            transform.rotation = Quaternion.LookRotation(moveDir);
        }

        rb.velocity = movement + Vector3.up * rb.velocity.y;    // include y-velocity in player movement

        LayerMask terrainMask = LayerMask.GetMask("Terrain");

        if (Physics.OverlapBox(transform.position - Vector3.up * 1, new Vector3(.5f, .05f, .5f), Quaternion.identity, terrainMask).Length > 0)
        {
            _isGrounded = true;
            if (currentState == PlayerState.JUMPING)
            {
                TransitionState(PlayerState.NORMAL);
            }
        }
        else
        {
            _isGrounded = false;
            if (currentState != PlayerState.LOCKINPUT)
            {
                TransitionState(PlayerState.JUMPING);
            }
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
                TransitionState(PlayerState.NORMAL);
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= runDelay)
            {
                TransitionState(PlayerState.RUNNING);
                timer = 0f;
            }
        }
    }

    public void SetMoveDir(Vector3 dir)
    {
        moveDir = dir;
    }

    public void SetMoveSpeed(float newSpeed)
    {
        movementMultiplier = newSpeed;
    }

    public void TogglePlayerUseGravity(bool gravityOn)
    {
        rb.useGravity = gravityOn;
        gravityModifier = (gravityOn) ? gravityMultiplier : 0f;
    }

    public void SetPhysicsLayer(int layer)
    {
        gameObject.layer = layer;
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.layer = layer;
            }
        }
    }

    public void ShowIndicator(bool isVisible)
    {
        dashIndicator.SetActive(isVisible);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position - Vector3.up * 1, new Vector3(.5f, .05f, .5f));
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawWireSphere(transform.position + transform.forward * (50 * .1f), 0.5f);
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            rb.velocity += Vector3.up * jumpSpeed;
        }
    }

    public void TransitionState(PlayerState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
        switch (currentState)
        {
            default:
            case PlayerState.NORMAL:
                movementMultiplier = moveSpeed;
                moveDir = (camForward * verticalMove + camRight * horizontalMove).normalized;
                break;
            case PlayerState.RUNNING:
                movementMultiplier = runSpeed;
                break;
            case PlayerState.JUMPING:
                movementMultiplier = jumpSpeed;
                break;
            case PlayerState.LOCKINPUT:
                break;
            case PlayerState.FREEZE:
                break;
        }

        GameController.instance.onPlayerStateChangedCallback.Invoke(currentState);
    }
}
