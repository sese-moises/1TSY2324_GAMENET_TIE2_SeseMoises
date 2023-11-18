using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    public float speed = 20;
    public float rotationSpeed = 200;
    public float currentSpeed = 0;

    public bool isControlEnabled;

    private void Start()
    {
        isControlEnabled = false;
        speed = 100;
    }

    private void LateUpdate()
    {
        if (isControlEnabled)
        {
            float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

            transform.Translate(0, 0, translation);
            currentSpeed = translation;

            transform.Rotate(0, rotation, 0);
        }
    }
}
