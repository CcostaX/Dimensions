using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentRoom : MonoBehaviour
{
    [SerializeField] private int currentRoom = 0;
    [SerializeField] private bool canChangeDimension = true;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gameManager.currentRoom = currentRoom;
            GameObject spawnPoint = transform.GetChild(0).gameObject;
            collision.gameObject.GetComponent<PlayerMovement>().currentSpawnPoint = spawnPoint;
            if (currentRoom >= 0)
                gameManager.currentRoomPosition = spawnPoint;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gameManager.currentRoom = currentRoom;
            GameObject spawnPoint = transform.GetChild(0).gameObject;
            collision.gameObject.GetComponent<PlayerMovement>().currentSpawnPoint = spawnPoint;
            if (currentRoom >= 0)
                gameManager.currentRoomPosition = spawnPoint;
        }
    }
}
