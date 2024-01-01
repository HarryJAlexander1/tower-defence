using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float MouseSensitivity;

    public Transform CameraTransform;

    private float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // make cursor invisible and locked on screen
    }

    // Update is called once per frame
    void Update()
    {
        // get mouse input
        var mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX); // rotate player on y axis as player moves mouse horizontally

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // stops player from rotating camera too far on x axis
        CameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // rotate camera on X axis
    }
}
