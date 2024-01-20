using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallrunForce;
    public float wallClimpSpeed;
    public float maxWallrunTime;
    private float wallrunTimer;

    [Header("Input")]
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
    public Transform oriantation;
    private PlayerMove pm;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForWalls();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if(pm.wallrunning)
        {
            WallrunningMovement();
        }
    }

    private void CheckForWalls()
    {
        wallRight = Physics.Raycast(transform.position, oriantation.right, out rightWallHit, wallCheckDistance, whatIsWall);//checks if there is a wall to the right
        wallLeft = Physics.Raycast(transform.position, -oriantation.right, out leftWallHit, wallCheckDistance, whatIsWall);//checks if there is a wall to the left
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHight, whatIsGround);//checks if the player is above ground
    }
    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
            if (!pm.wallrunning)
            {
                StartWallrun();
            }
        }
        else
        {
            if(pm.wallrunning)
            {
                StopWallrun();
            }
        }
    }
    private void StartWallrun()
    {
        pm.wallrunning = true;
    }
    private void WallrunningMovement()
    {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((oriantation.forward - wallForward).magnitude > (oriantation.forward - -wallForward).magnitude)//cheks if the wall is to the right or left of the player and changes the direction of the wallrunning acordingly
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * wallrunForce, ForceMode.Force);

        if(upwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, wallClimpSpeed, rb.velocity.z);
        }
        if (downwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallClimpSpeed, rb.velocity.z);
        }

        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);//pushes the player to the wall
        }
        
    }
    private void StopWallrun()
    {
        pm.wallrunning = false;
    }
}
