using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallrunForce;
    public float wallJumpUpForce;
    public float walljumpSideForce;
    public float wallClimpSpeed;
    public float maxWallrunTime;
    private float wallrunTimer;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
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

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("References")]
    public Transform oriantation;
    public PlayerCam cam;
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

        //Wallrunning state
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.wallrunning)
            {
                StartWallrun();
            }

            if(wallrunTimer > 0)
            {
                wallrunTimer -= Time.deltaTime;
            }

            if (wallrunTimer <= 0)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            if (Input.GetKeyDown(jumpKey))
            {
                WallJump();
            }
        }

        //Exiting state
        else if (exitingWall)
        {
            if (pm.wallrunning)
            {
                StopWallrun();
            }

            if(exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if(exitWallTimer < 0)
            {
                exitingWall = false;
            }
        }

        //None state
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
        cam.DoFov(70f);

        pm.wallrunning = true;

        wallrunTimer = maxWallrunTime;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

    }
    private void WallrunningMovement()
    {
        rb.useGravity = useGravity;

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

        if (useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }

        

    }
    private void StopWallrun()
    {
        pm.wallrunning = false;

        cam.DoFov(60f);

    }

    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * walljumpSideForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
