using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    private float fov;
    public float fovSpeed;
    private float targetFov;

    private float tilt;
    public float tiltSpeed;
    private float targetTilt;

    public float sensX;
    public float sensY;

    public Transform camHolder;
    public Transform orientation;

    float xRotation;
    float yRotation;
    // Start is called before the first frame update
    void Start()
    {
        fov = GetComponent<Camera>().fieldOfView;
        targetFov = GetComponent<Camera>().fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, - 90, 90);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        GetComponent<Camera>().fieldOfView = fov;
        if (targetFov > fov)
        {
            fov += fovSpeed * Time.deltaTime;
        }
        if (targetFov < fov)
        {
            fov -= (fovSpeed * Time.deltaTime) ;
        }
    }

    public void DoFov(float newTargetFov)
    {
        targetFov = newTargetFov;
    }
}
