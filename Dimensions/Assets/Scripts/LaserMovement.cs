using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMovement : MonoBehaviour
{
    private GameManager gameManager;
    public float verticalSpeed = 5f;
    public float upDownRange = 5f;

    public bool isVertical = true;
    public bool isUp = true;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.currentRoom == 5)
        {
            StartCoroutine(MoveLaser());
        }
    }

    IEnumerator MoveLaser()
    {
        if (isVertical)
        {
            //vertical
            float startY = transform.position.y;
            while (true)
            {
                float newY = startY + Mathf.PingPong(Time.time * verticalSpeed, upDownRange * 2);
                if (isUp)
                    newY = -newY;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                yield return null;
            }
        }
        else
        {
            //horizontal
            float startZ = transform.position.z;
            while (true)
            {
                float newZ = startZ + Mathf.PingPong(Time.time * verticalSpeed, upDownRange * 2);
                if (!isUp)
                    newZ = -newZ;
                transform.position = new Vector3(transform.position.x, transform.position.y, -newZ);
                yield return null;
            }
        }
    }
}
