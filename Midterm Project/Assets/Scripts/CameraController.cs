using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float sensitivity;
    [SerializeField] float lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false; // turn the cursor off
        Cursor.lockState = CursorLockMode.Locked; // lock the cursor in center
    }

    // Update is called once per frame
    void Update()
    {
        // get input
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        // invertion
        if (invertY) mouseY = -mouseY;

        // rotate
        rotX -= mouseY;

        // clamp pitch
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // rotate camera pitch
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // rotate player yaw
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
