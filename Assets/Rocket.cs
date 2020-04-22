using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>(); // Unity provided function. Gets all components of type RigidBody within context
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up); // Position of the ship is a vector three, three floating point numbers
        }

        if (Input.GetKey(KeyCode.A))
        {
            print("Rotating left");

        } else if (Input.GetKey(KeyCode.D))
        {
            print("Rotating right");
        }
    }
}
