using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour
{
    public GameObject button;
    GameObject presser;
    AudioSource sound;
    bool isPressed;

    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isPressed)
        {
            sound.Play();
            button.transform.localPosition = new Vector3(button.transform.localPosition.x, button.transform.localPosition.y, 0.07f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0.07f);
            presser = collision.collider.gameObject;
            isPressed = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag == "Player")
        {
            button.transform.localPosition = new Vector3(button.transform.localPosition.x, button.transform.localPosition.y, -0.05f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -0.05f);
            isPressed = false;
        }
    }


    public void SpawnSphere()
    {
        Debug.Log("Pressed");
    }
}
