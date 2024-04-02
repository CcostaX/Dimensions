using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour
{
    public GameObject buttonPress;
    AudioSource sound;
    public bool isPressed = false;
    [SerializeField] private GameObject door;
    [SerializeField] private int currentRoom = -1;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        //verify if all buttons are pressed (2D and 3D)
        if (!isPressed && (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Object3D"))
        {
            //transform pressed button to simulate press on button
            buttonPress.transform.localPosition = new Vector3(buttonPress.transform.localPosition.x, buttonPress.transform.localPosition.y, 0.07f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0.07f);
            verifyPressedButtons();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (isPressed && (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Object3D"))
        {
            buttonPress.transform.localPosition = new Vector3(buttonPress.transform.localPosition.x, buttonPress.transform.localPosition.y, -0.05f);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -0.05f);
            isPressed = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //verify if all buttons are pressed (2D and 3D)
        if (!isPressed && (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Object2D"))
        {
            //Change press color
            Color newColor;
            if (ColorUtility.TryParseHtmlString("#3A792E", out newColor)) //dark green
            {
                SpriteRenderer rendererComponent = buttonPress.GetComponent<SpriteRenderer>();
                if (rendererComponent != null)
                {
                    rendererComponent.material.color = newColor;
                }
            }

            verifyPressedButtons();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isPressed && (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Object2D"))
        {
            //Change press color
            Color newColor;
            if (door.activeSelf && ColorUtility.TryParseHtmlString("#8BFF77", out newColor)) //green
            {
                SpriteRenderer rendererComponent = buttonPress.GetComponent<SpriteRenderer>();
                if (rendererComponent != null)
                {
                    rendererComponent.material.color = newColor;
                }
            }

            isPressed = false;
        }
    }

    private void verifyPressedButtons()
    {
        sound.Play();
        isPressed = true;

        GameObject[] buttons = GameObject.FindGameObjectsWithTag("ButtonPress");
        int buttonPressedCount = 0;
        int buttonInRoomCount = 0;
        foreach (GameObject button in buttons)
        {
            if (button.GetComponent<ButtonScript>().currentRoom == gameManager.currentRoom)
            {
                buttonInRoomCount++;
                if (button.GetComponent<ButtonScript>().isPressed)
                {
                    buttonPressedCount++;
                }
            }
        }

        if (buttonPressedCount == buttonInRoomCount)
        {
            //Disable all buttons pressed
            foreach (GameObject button in buttons)
            {
                if (button.GetComponent<ButtonScript>().isPressed)
                {
                    if (currentRoom == gameManager.currentRoom)
                    {
                        door.SetActive(false);
                        button.SetActive(false);
                    }
                }
            }
        }
    }
}
