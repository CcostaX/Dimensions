using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour
{
    public GameObject button;
    AudioSource sound;
    public bool isPressed = false;
    [SerializeField] private GameObject door;

    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!isPressed && (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Object3D"))
        {
            sound.Play();
            button.transform.localPosition = new Vector3(button.transform.localPosition.x, button.transform.localPosition.y, 0.07f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0.07f);
            isPressed = true;

            GameObject[] buttons = GameObject.FindGameObjectsWithTag("ButtonPress");
            int buttonPressedCount = 0;
            foreach (GameObject button in buttons)
            {
                if (button.GetComponent<ButtonScript>().isPressed)
                {
                    buttonPressedCount++;
                }
            }
            if (buttonPressedCount == buttons.Length)
            {
                //Disable all buttons pressed
                foreach (GameObject button in buttons)
                {
                    if (button.GetComponent<ButtonScript>().isPressed)
                    {
                        button.SetActive(false);
                        door.SetActive(false);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (isPressed && (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Object3D"))
        {
            button.transform.localPosition = new Vector3(button.transform.localPosition.x, button.transform.localPosition.y, -0.05f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -0.05f);
            isPressed = false;
        }
    }
}
