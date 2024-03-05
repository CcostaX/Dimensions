using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMovement : MonoBehaviour
{
    private GameManager gameManager;
    public float verticalSpeed = 5f;
    public float upDownRange = 5f;

    public bool isMoving = true;
    public bool isVertical = true;
    public bool isUp = true;
    public bool isRotated = false;
    private bool isLaserRoom = false;
    private Vector3 initialPosition;
    private float laserTime;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLaserRoom && gameManager.currentRoom == 5)
        {
            isLaserRoom = true;
            if (isMoving)
                StartCoroutine(MoveLaser());
        }
        else if (isLaserRoom && gameManager.currentRoom != 5)
        {
            isLaserRoom = false;
        }
    }

    IEnumerator MoveLaser()
    {
        laserTime = 0f;
        //initialPosition = transform.position;

        while (isLaserRoom)
        {
            float movement = Mathf.Sin(laserTime * verticalSpeed) * upDownRange;


            if (isRotated)
            {
                if (!isUp)
                    movement = -movement;
                transform.position = new Vector3(initialPosition.x + movement, initialPosition.y, initialPosition.z);
            }
            else
            {
                if (isVertical)
                {
                    if (!isUp)
                        movement = -movement;
                    transform.position = new Vector3(initialPosition.x, initialPosition.y + movement, initialPosition.z);
                }
                else
                {
                    if (isUp)
                        movement = -movement;
                    transform.position = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z + movement);
                }
            }


            // Increment the time progression
            laserTime += Time.deltaTime;

            yield return null;
        }
    }   
}
