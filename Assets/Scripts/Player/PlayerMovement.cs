using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController CharacterController;
    public float Speed; // movement speed variable
    private float SprintingSpeed;
    private float WalkingSpeed;
    private float Gravity = -9.81f;
    Vector3 Velocity;

    // variables to check if player is grounded (not falling)
    public Transform GroundCheckTransform;
    private float GroundDistance = 0.4f;
    public LayerMask GroundMask;
    private bool IsGrounded = false;

    private float JumpHeight = 5f;

    private Vector3 StandingScale;
    private Vector3 CrouchingScale;

    private Vector3 PreviousPosition;
    private bool IsMoving;
    public GameObject Head;

    bool CameraMovingUp;
    // Start is called before the first frame update
    void Start()
    {
        SprintingSpeed = Speed * 1.5f;
        WalkingSpeed = SprintingSpeed * 0.5f;
        StandingScale = transform.localScale;
        CrouchingScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.5f, transform.localScale.z);
        PreviousPosition = transform.position;
        CameraMovingUp = true;
    }

    // Update is called once per frame
    void Update()
    {
       HandleMovement();   
    }

    private void HandleMovement() 
    {
        IsGrounded = Physics.CheckSphere(GroundCheckTransform.position, GroundDistance, GroundMask); // check to see if player is on the ground

        if (IsGrounded && Velocity.y < 0f)
        {
            Velocity.y = -2f; // setting players velocity on y axis to -2f and not 0 to account for delay in call
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // logic for moving player along x and z axis
        Vector3 move = transform.right * x + transform.forward * z;
        CharacterController.Move(move * Speed * Time.deltaTime);

        // simulate gravity acting on player
        Velocity.y += Gravity * Time.deltaTime;
        CharacterController.Move(Velocity * Time.deltaTime);

        // logic for jumping on y axis
        if (IsGrounded && Input.GetButtonDown("Jump"))
        {
            Velocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        // logic for sprinting
        if (IsGrounded && Input.GetKey(KeyCode.LeftShift))
        {
            Speed = SprintingSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift)) 
        {
            Speed = WalkingSpeed;
        }

        // logic for couching
        if (IsGrounded && Input.GetKey(KeyCode.LeftControl))
        {
            transform.localScale = CrouchingScale;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            transform.localScale = StandingScale;
        }

        if (transform.position != PreviousPosition)
        {
            // The parent GameObject is moving.
            Debug.Log("Player is moving!");
            IsMoving = true;
            // Update the previous position for the next frame.
            PreviousPosition = transform.position;
        }
        else
        {
            Debug.Log("Player is stationary");
            IsMoving = false;
        }

        if (IsMoving && IsGrounded) 
        {
            // bob camera to simulate walking
            BobHead(0.8f);
        }
    }

    private void BobHead(float speed) 
    {
        float maxY = 0.6f;
        float minY = 0.4f;

        float moveSpeed = speed;
        
        // Check if the object is moving up
        if (CameraMovingUp)
        {
            // Move the object up
            Head.transform.Translate(moveSpeed * Time.deltaTime * Vector3.up);

            // Check if the object has reached or exceeded the maximum y position
            if (Head.transform.localPosition.y >= maxY)
            {
                CameraMovingUp = false; // Switch to moving down
            }
        }
        else
        {
            // Move the object down
            Head.transform.Translate(moveSpeed * Time.deltaTime * Vector3.down);

            // Check if the object has reached or gone below the minimum y position
            if (Head.transform.localPosition.y <= minY)
            {
                CameraMovingUp = true; // Switch to moving up
            }
        }
    }
}
