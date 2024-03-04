using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.parent != null && collision.transform.parent.tag == "Player")
        {
            //Spawn in the beggining of the room
            GameObject player = collision.transform.parent.gameObject;
            collision.transform.parent.gameObject.transform.position = player.GetComponent<PlayerMovement>().currentSpawnPoint.transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent != null && collision.transform.parent.tag == "Player")
        {
            //Spawn in the beggining of the room
            GameObject player = collision.transform.parent.gameObject;
            collision.transform.parent.gameObject.transform.position = player.GetComponent<PlayerMovement>().currentSpawnPoint.transform.position;
        }
    }
}
