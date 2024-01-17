using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    public Transform target;  // The player's transform
    public bool isDimension2D = true;
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
            //Change Dimension to 2.5D
            camera.orthographic = false;
            camera.fieldOfView = 14;
            transform.rotation = Quaternion.Euler(-45, transform.rotation.y, transform.rotation.z);

            GameObject player =  GameObject.FindGameObjectWithTag("Player");
            Debug.Log(player);
            StartCoroutine(ChangePlayerCollider(false, player));
        }
        else
        {
            //Change Dimension to 2D
            camera.orthographic = true;
            camera.orthographicSize = 4;
            transform.rotation = Quaternion.Euler(0, transform.rotation.y, transform.rotation.z);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Debug.Log(player);
            StartCoroutine(ChangePlayerCollider(true, player));
        }
    }

    IEnumerator ChangePlayerCollider(bool isPlayerDimension2D, GameObject player)
    {
        if (isPlayerDimension2D)
        {
            //change BoxCollider to BoxCollider2D
            Destroy(player.GetComponent<BoxCollider>());
            Destroy(player.GetComponent<Rigidbody>());

            // Wait for the next frame
            yield return null;

            BoxCollider2D collider2D = player.AddComponent<BoxCollider2D>();
            collider2D.size = new Vector2(1f, 1f);
            Rigidbody2D rigidbody2D = player.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0;
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            isDimension2D = true;
        }
        else
        {
            //change BoxCollider2D to BoxCollider
            Destroy(player.GetComponent<BoxCollider2D>());
            Destroy(player.GetComponent<Rigidbody2D>());

            // Wait for the next frame
            yield return null;

            BoxCollider collider = player.AddComponent<BoxCollider>();
            collider.size = new Vector3(1f, 1f, 1f);
            Rigidbody rigidbody = player.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            rigidbody.useGravity = false;

            isDimension2D = false;
        }

    }
}
