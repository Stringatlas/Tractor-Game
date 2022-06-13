using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float moveSpeed;
    public float gravity = -9.81f;
    private float normalGravity;

    public float jumpHeight = 7f;

    public Transform groundCheck;
    public float groundDistance = 10f;
    public LayerMask groundMask;
    bool isGrounded;

    [SerializeField] float sprintingMultiplier;
    [SerializeField] float gravityMultiplier;

    Vector3 velocity;

    float normalMoveSpeed;

    void Start()
    {
        normalGravity = gravity;
        normalMoveSpeed = moveSpeed;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < -2)
        {
            velocity.y = -2;
        }


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (Input.GetAxisRaw("Sprint") == 1)
        {
            moveSpeed = normalMoveSpeed * sprintingMultiplier;
        }
        else
        {
            moveSpeed = normalMoveSpeed;
        }

        if ((Input.GetAxisRaw("Sneak") == 1) && isGrounded)
        {
            gravity = gravity * gravityMultiplier;
        }
        else
        {
            gravity = normalGravity;
        }
    }
}