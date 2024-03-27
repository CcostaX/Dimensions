using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    [SerializeField] private GameObject[] players = new GameObject[2];
    public bool isDimension2D = true;
    private GameObject groundCheckObject = null;
    [SerializeField] private Camera camera;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject canvas_battlezone;
    [SerializeField] private GameObject canvas_screen;
    [SerializeField] private Light gameLight;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Find and assign players
        GameObject player2D = GameObject.Find("Player2D");
        GameObject player3D = GameObject.Find("Player3D");

        // Check if both players were found
        if (player2D != null && player3D != null)
        {
            players[0] = player2D;
            players[1] = player3D;
        }
        else
        {
            Debug.LogError("One or more players not found!");
        }
    }

    void Update()
    {
        if (players[0] != null && players[1] != null)
        {
            // Set the camera's position to match the player's position
            if (camera.orthographic == false)
            {
                //Dimension 2.5D
                transform.position = new Vector3(players[1].transform.position.x, players[1].transform.position.y - 18, players[1].transform.position.z - 18);
                players[0].transform.position = new Vector3(players[0].transform.position.x, players[0].transform.position.y, players[1].transform.position.z);
            }
            else
            {
                //Dimension 2D
                transform.position = new Vector3(players[0].transform.position.x, players[0].transform.position.y, transform.position.z);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && gameManager.finishBattleScreenAnimation) //Jump
        {
            ChangeCameraTo2D();
            ChangePlayer2DAltitude();
        }

    }


    public IEnumerator CameraEnemy_BattleZone(Transform enemy)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = camera.transform.position;
        Vector3 targetPos = new Vector3(enemy.position.x, enemy.position.y, camera.transform.position.z);

        //Change text and color
        TextMeshProUGUI monsterText = canvas_screen.transform.Find("MonsterText").GetComponent<TextMeshProUGUI>();
        if (ColorUtility.TryParseHtmlString("#16A12E", out Color myColor)) //Dark Green
        {
            monsterText.color = myColor;
        }

        // Camera View on enemy
        while (elapsedTime < 2f)
        {
            if (camera.orthographic)
            {
                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, 3f, elapsedTime / 2f);
                camera.transform.position = Vector3.Lerp(startingPos, targetPos, elapsedTime * 7);
            }
            else
            {
                camera.transform.position = Vector3.Lerp(startingPos, new Vector3(targetPos.x, targetPos.y - 8, targetPos.z + 10), elapsedTime*7);
            }


            monsterText.text = "Green Slime";
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Move back to the original position
        elapsedTime = 0f;
        monsterText.text = "";

        while (elapsedTime < 0.2f)
        {
            if (camera.orthographic)
            {
                camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, 6f, elapsedTime / 2f);
                camera.transform.position = Vector3.Lerp(targetPos, startingPos, elapsedTime * 7);
            }
            else
            {
                camera.transform.position = Vector3.Lerp(
                    new Vector3(targetPos.x, targetPos.y - 8, targetPos.z + 10),
                    startingPos,
                    elapsedTime * 7
                );

            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvas_battlezone.SetActive(true);
        gameManager.finishBattleScreenAnimation = true;

        yield return null;
    }

    public void ChangeCameraTo2D()
    {
        if (isDimension2D)
        {
            //Change Dimension to 2.5D
            camera.orthographic = false;
            camera.fieldOfView = 17;
            transform.rotation = Quaternion.Euler(-45, transform.rotation.y, transform.rotation.z);

            //GameObject player =  GameObject.FindGameObjectWithTag("Player");
            //player.GetComponent<PlayerMovement>().moveSpeed = 5f;
            //StartCoroutine(ChangePlayerCollider(false, player));
            StartCoroutine(ChangeEnemyCollider(false));

            isDimension2D = false;
            //Change current character
            gameManager.currentPlayerInControl = GameObject.Find("Player3D");
            //stop 3D character movement
            GameObject.Find("Player2D").GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
        }
        else
        {
            //Change Dimension to 2D
            camera.orthographic = true;
            camera.orthographicSize = 6;
            transform.rotation = Quaternion.Euler(0, transform.rotation.y, transform.rotation.z);

            //GameObject player = GameObject.FindGameObjectWithTag("Player");
            //player.GetComponent<PlayerMovement>().moveSpeed = 5f;
            //StartCoroutine(ChangePlayerCollider(true, player));
            StartCoroutine(ChangeEnemyCollider(true));

            isDimension2D = true;
            //Change current character
            gameManager.currentPlayerInControl = GameObject.Find("Player2D");
            //stop 3D character movement
            GameObject.Find("Player3D").GetComponent<Rigidbody>().velocity = new Vector2(0, 0);
        }
    }

    IEnumerator ChangePlayerCollider(bool isPlayerDimension2D, GameObject player)
    {
        if (isPlayerDimension2D)
        {
            //change BoxCollider to BoxCollider2D
            Destroy(player.GetComponent<BoxCollider>());
            Destroy(player.GetComponent<Rigidbody>());

            ChangeObject3D(false);
            //ChangeObject2D(false);

            // Wait for the next frame
            yield return null;

            Destroy(groundCheckObject);
            BoxCollider2D collider2D = player.AddComponent<BoxCollider2D>();
            collider2D.size = new Vector2(1f, 1f);
            Rigidbody2D rigidbody2D = player.AddComponent<Rigidbody2D>();
            rigidbody2D.gravityScale = 0;
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            isDimension2D = true;
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10f);
        }
        else
        {
            //change BoxCollider2D to BoxCollider
            Destroy(player.GetComponent<BoxCollider2D>());
            Destroy(player.GetComponent<Rigidbody2D>());

            ChangeObject3D(true);
            //ChangeObject2D(true);

            // Wait for the next frame
            yield return null;

            BoxCollider collider = player.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.5f, 0.5f, 0.5f);
            collider.material.dynamicFriction = 0;
            collider.material.staticFriction = 0;
            Rigidbody rigidbody = player.AddComponent<Rigidbody>();
            rigidbody.mass = 0.6f;
            rigidbody.drag = 4;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            // Add a Ground Check object
            groundCheckObject = new GameObject("GroundCheck");
            groundCheckObject.transform.position = player.transform.position;
            groundCheckObject.transform.localScale = new Vector3(1f, 1f, 1f);
            groundCheckObject.transform.parent = player.transform;

            isDimension2D = false;
            RaycastHit hit;
            if (Physics.Raycast(groundCheckObject.transform.position, Vector3.forward, out hit))
            {
                // Calculate the position slightly above the detected object along the normal
                Vector3 newPosition = hit.point + hit.normal * 0.7f;
                Debug.Log(player.transform.position.z + " " + newPosition.z);
                // Set the character's position to the new position
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, newPosition.z);
            }
        }
    }

    IEnumerator ChangeEnemyCollider(bool isPlayerDimension2D)
    {
        //Set enemies in battlezone true
        GameObject enemiesInBattleZone = GameObject.Find("BattleZone/Enemies");
        foreach (Transform child in enemiesInBattleZone.transform)
        {
            child.gameObject.SetActive(true);
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            Debug.Log(enemy);
            if (isPlayerDimension2D)
            {
                //change BoxCollider to BoxCollider2D
                Destroy(enemy.GetComponent<BoxCollider>());
                Destroy(enemy.GetComponent<Rigidbody>());

                // Wait for the next frame
                yield return null;

                BoxCollider2D collider2D = enemy.AddComponent<BoxCollider2D>();
                collider2D.size = new Vector2(1f, 1f);
                Rigidbody2D rigidbody2D = enemy.AddComponent<Rigidbody2D>();
                rigidbody2D.gravityScale = 0;
                rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                isDimension2D = true;
                enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y, -10f);
            }
            else
            {
                //change BoxCollider2D to BoxCollider
                Destroy(enemy.GetComponent<BoxCollider2D>());
                Destroy(enemy.GetComponent<Rigidbody2D>());
                Destroy(groundCheckObject);
                // Wait for the next frame
                yield return null;


                BoxCollider collider = enemy.AddComponent<BoxCollider>();
                collider.size = new Vector3(0.5f, 0.5f, 0.5f);
                collider.material.dynamicFriction = 0;
                collider.material.staticFriction = 0;
                Rigidbody rigidbody = enemy.AddComponent<Rigidbody>();
                rigidbody.mass = 0.6f;
                rigidbody.drag = 4;
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                // Add a Ground Check object
                groundCheckObject = new GameObject("GroundCheck");
                groundCheckObject.transform.position = enemy.transform.position;
                groundCheckObject.transform.localScale = new Vector3(1f, 1f, 1f);
                groundCheckObject.transform.parent = enemy.transform;

                isDimension2D = false;
                RaycastHit hit;
                if (Physics.Raycast(groundCheckObject.transform.position, Vector3.forward, out hit))
                {
                    // Calculate the position slightly above the detected object along the normal
                    Vector3 newPosition = hit.point + hit.normal * 0.7f;
                    Debug.Log(enemy.transform.position.z + " " + newPosition.z);
                    // Set the character's position to the new position
                    enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y, newPosition.z);
                }
            }
        }

       /* if (gameManager.currentRoom < 0)
        {
            foreach (Transform child in enemiesInBattleZone.transform)
            {
                child.gameObject.SetActive(false);
            }
        }*/
    }

    private void ChangeObject3D(bool isPlayerDimension3D)
    {
        //Enable/Disable floor collision for 3D
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        foreach (GameObject floor in floors)
        {
            floor.GetComponent<BoxCollider>().enabled = isPlayerDimension3D;
        }

        GameObject[] objects3d = GameObject.FindGameObjectsWithTag("Object3D");
        foreach (GameObject object3D in objects3d)
        {
            Rigidbody rbObject = object3D.GetComponent<Rigidbody>();
            if (rbObject != null && isPlayerDimension3D)
            {
                rbObject.useGravity = true;
            }
            else if (rbObject != null && !isPlayerDimension3D)
            {
                rbObject.useGravity = false;

            }
        }
    }

    private void ChangePlayer2DAltitude()
    {
        if (camera.orthographic)
        {
            //Camera 2d
            players[0].transform.position = new Vector3(players[0].transform.position.x, players[0].transform.position.y, -10);
        }
        else
        {
            //Camera 2.5d
            players[0].transform.position = new Vector3(players[0].transform.position.x, players[0].transform.position.y, players[1].transform.position.z);
        }
        
    }
}
