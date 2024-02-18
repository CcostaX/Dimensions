using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    public Transform target;  // The player's transform
    public float smoothness = 5.0f;

    void Update()
    {
        if (target != null)
        {
            // Set the camera's position to match the player's position
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothness * Time.deltaTime);
        }
    }
}
