using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMove : MonoBehaviour
{

    public TextMeshProUGUI speedText;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        speedText.text = ("Speed: " + rb.velocity.magnitude.ToString("F1"));

        grounded = Physics.Raycast(transform.position, Vector2.down, playerHeight * 0.5f + 0.2f, whatIsGround);//grounded is true if the raycast looking for whatIsGround layer is hitting ground

        if (grounded)//apply drag when grounded
        {
            rb.drag = groundDrag;
        }
        else//remove drag when airborn
        {
            rb.drag = 0;
        }

        PlayerInput();
        SpeedControl();
        StateHandler();
    }
    private void FixedUpdate()
    {
        movePlayer();
    }
    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if(Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);//make the player shorter when crouching
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);//add force down so the player isn't floating
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);//make the player normal hight when not crouching
        }
    }
    private void StateHandler()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }
    private void movePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);//ands a force along the slope acording to the slope angle

            if (rb.velocity.y > 0 )
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();//disables gravity if player is on a slope, so the player dose not slide while on a slope
    }
    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
            
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)//caps the speed at move speed
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

    }
    private void Jump()
    {
        exitingSlope = true;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);//resets the velocity

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);//add force up
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))//makes a raykast to get information of the fround the player is standing on
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);//calculates the angle of the object the raycast hits
            return angle < maxSlopeAngle && angle != 0;//return on slope as true if the angle of the ground below the player isn't 0 and is less than the max slope angle
        }
        
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;//projects the normal move direction onto the slope
    }
}
