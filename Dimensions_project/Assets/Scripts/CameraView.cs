using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    public Transform target;  // The player's transform
    private bool isDimension2D = true;
    [SerializeField] private Camera camera;

    void Start()
    {
    }

    void Update()
    {
        if (target != null)
        {
            // Set the camera's position to match the player's position
            if (camera.orthographic == false)
            {
                //Dimension 2.5D
                transform.position = new Vector3(target.position.x, target.position.y - 20, transform.position.z);
            }
            else
            {
                //Dimension 2D
                transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
            }
        }
    }

    public void ChangeCameraTo2D()
    {
        if (isDimension2D)
        {
            //Dimension 2.5D
            isDimension2D = false;
            camera.orthographic = false;
            camera.fieldOfView = 14;
            transform.rotation = Quaternion.Euler(-45, transform.rotation.y, transform.rotation.z);
        }
        else
        {
            //Dimension 2D
            isDimension2D = true;
            camera.orthographic = true;
            camera.orthographicSize = 4;
            transform.rotation = Quaternion.Euler(0, transform.rotation.y, transform.rotation.z);
        }
    }
}
