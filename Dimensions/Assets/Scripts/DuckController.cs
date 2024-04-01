using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckController : MonoBehaviour
{
    public float swimSpeed = 0.2f;
    public float turnSpeed = 15f;

    void Start()
    {

    }

    void Update()
    {
        transform.Rotate(Vector3.down * turnSpeed * Time.deltaTime);
        transform.Translate(Vector3.forward * swimSpeed * Time.deltaTime);
    }
}
