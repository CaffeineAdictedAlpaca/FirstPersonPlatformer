using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMove pm;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;

    public float slideYScale;
    private float startYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMove>();

        startYScale = playerObj.localScale.y;
    }
    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))//start slide if entering slide key while moving
        {
            StartSlide();
        }
        if(Input.GetKeyUp(slideKey) && pm.sliding)//stops slide if let go of slide key and is sliding
        {
            StopSlide();
        }
    }
    private void FixedUpdate()
    {
        if(pm.sliding)//calls sliding movement when sliding
        {
            SlidingMovement();
        }
    }
    private void StartSlide()
    {
        pm.sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);//makes player short
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);//puts the player on the ground

        slideTimer = maxSlideTime;//resets the slide timer
    }
    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;//makes it posible to slide in all directions

        if(!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);//adds force in the direction the player is sliding

            slideTimer -= Time.deltaTime;//counts down the slide timer when sliding
        }

        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);//adds force in the direction the player is sliding
        }

        if(slideTimer <= 0)//stop sliding when slide timer is 0
        {
            StopSlide();
        }
    }
    private void StopSlide()
    {
        pm.sliding = false;

        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);//makes player normal hight
    }

}
