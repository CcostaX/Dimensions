using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    public Transform target;  // The player's transform

    void Update()
    {
        if (target != null)
        {
            // Set the camera's position to match the player's position
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}
