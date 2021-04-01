using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 15f;
    [SerializeField] float deadZone = .1f;

    public bool isGrounded { get { return _isGrounded; } }
    bool _isGrounded = false;

    float timer = 0f;

    Rigidbody rb;
    Camera cam;
    Joystick joystick;

    float horizontalMove;
    float verticalMove;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        joystick = GameObject.Find("Fixed Joystick").GetComponent<Joystick>();
        joystick.DeadZone = deadZone;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = joystick.Horizontal * moveSpeed;
        verticalMove = joystick.Vertical * moveSpeed;

        Vector3 camForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
        Vector3 camRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;

        rb.velocity = camForward * verticalMove + camRight * horizontalMove;

        timer += Time.deltaTime;
        if (timer >= 3f)
        {
            timer = 0f;
        }
    }
}
