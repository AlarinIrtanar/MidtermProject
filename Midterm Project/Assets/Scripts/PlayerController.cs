using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] int maxJumps;
    [SerializeField] float gravity;

    Vector3 moveDir;
    Vector3 velocity;
    int timesJumped;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    /// <summary>
    /// Does a tick of player movement
    /// </summary>
    void Movement()
    {
        if (controller.isGrounded)
        {
            timesJumped = 0;
            velocity.y = 0;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && timesJumped < maxJumps)
        {
            timesJumped++;
            velocity.y = jumpForce;
        }

        velocity.y -= (gravity / 2) * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        velocity.y -= (gravity / 2) * Time.deltaTime;
    }
}
