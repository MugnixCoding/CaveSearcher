using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform cam;

    [Header("Mouse Sensitive")]
    [Range(1,100)]
    [SerializeField] private float xSensitivity = 50f;
    [Range(1, 100)]
    [SerializeField] private float ySensitivity = 50f;



    float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }
    // Update is called once per frame
    void Update()
    {
        Vector2  mouseVector = InputManager.Instance.GetMouseValue();
        xRotation -= mouseVector.y * Time.deltaTime * xSensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.Rotate(Vector3.up * (mouseVector.x * Time.deltaTime) * ySensitivity);
    }
    public void SetCameraHeight(float newHeight)
    {
        cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, newHeight, cam.transform.localPosition.z);
    }
    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
