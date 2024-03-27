using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public GameObject currentPlayerCircle;
    private float CirclePositionZ;
    [SerializeField] private GameObject[] players = new GameObject[2];
    public int currentRoom = 0;
    public GameObject currentRoomPosition;
    public GameObject currentPlayerInControl;
    [Header("Lives")]
    public int lives = 5;
    public int maxLives = 5;
    [SerializeField] private GameObject livePrefab;
    [SerializeField] private GameObject livePrefabPlaceholder;
    private GameObject background_lives_placeholder;
    private GameObject background_lives;
    [SerializeField] private GameObject canvas_battlezone;
    [SerializeField] private GameObject canvas_screen;
    public bool finishBattleScreenAnimation = true;
    public Vector2 originalPlayerPosition2D;
    public Vector3 originalPlayerPosition3D;
    public GameObject currentEnemyHit;

    [SerializeField] private CameraView cameraView;
    // Start is called before the first frame update
    void Start()
    {
        //Lives
        background_lives_placeholder = GameObject.Find("Canvas/Background_lives/Lives_placeholder");
        background_lives = GameObject.Find("Canvas/Background_lives/Lives");
        int livePositionX = 230;
        for (int i = 0; i < maxLives; i++)
        {
            //Instantiate player's lives placeholder
            Instantiate(livePrefabPlaceholder, new Vector3(livePrefab.transform.position.x + livePositionX, background_lives_placeholder.transform.position.y, background_lives_placeholder.transform.position.z), Quaternion.identity, background_lives_placeholder.transform);           
            livePositionX += 100;
        }
        livePositionX = 230;
        for (int i = 0; i < lives; i++)
        {
            //Instantiate player's lives
            Instantiate(livePrefab, new Vector3(livePrefab.transform.position.x + livePositionX, background_lives.transform.position.y, background_lives.transform.position.z), Quaternion.identity, background_lives.transform);
            livePositionX += 100;
        }
        //OTHER
        cameraView = GameObject.Find("MainCamera").GetComponent<CameraView>();
        originalPlayerPosition2D = new Vector2(0, 0);
        originalPlayerPosition3D = new Vector3(0, 0, 0);

        //Players
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

    // Update is called once per frame
    void Update()
    {
        if (currentPlayerInControl == players[0])
            currentPlayerCircle.transform.position = new Vector3(players[0].transform.position.x, players[0].transform.position.y, players[0].transform.position.z);
        else if (currentPlayerInControl == players[1])
        {
            if (players[1].GetComponent<PlayerMovement>().canJump)
            {
                CirclePositionZ = players[1].transform.position.z;
                currentPlayerCircle.transform.position = new Vector3(players[1].transform.position.x, players[1].transform.position.y-0.5f, players[1].transform.position.z);
            }
            else
                currentPlayerCircle.transform.position = new Vector3(players[1].transform.position.x, players[1].transform.position.y-0.5f, CirclePositionZ);
        }
  
    }

    public IEnumerator ScreenAnimation_BattleZone(Vector3 spawnPoint)
    {
        //Obtain player position for going back
        originalPlayerPosition2D = players[0].transform.position;
        originalPlayerPosition3D = players[1].transform.position;

        yield return StartCoroutine(ScreenAnimation(spawnPoint));

        //find enemy in battle zone and start enemy camera view
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyMovement>().isBattleMode)
            {
                yield return StartCoroutine(cameraView.CameraEnemy_BattleZone(enemy.transform));
                break;
            }
        }

        Debug.Log("Finish animation");

        yield return null;
    }

    public IEnumerator ScreenAnimation_Home(Vector3 spawnPoint)
    {
        yield return StartCoroutine(ScreenAnimation(spawnPoint));
        yield return StartCoroutine(PlayerInvisibility());
        yield return null;
    }

    IEnumerator PlayerInvisibility()
    {
        //resume player movement
        finishBattleScreenAnimation = true;
        players[0].GetComponent<PlayerMovement>().StopPlayer(false);
        players[1].GetComponent<PlayerMovement>().StopPlayer(false);

        //Player invisibility color change
        for (int i = 0; i < 10; i++)
        {
            players[0].GetComponent<SpriteRenderer>().color = Color.grey;
            players[1].GetComponent<SpriteRenderer>().color = Color.grey;
            yield return new WaitForSeconds(0.15f);
            players[0].GetComponent<SpriteRenderer>().color = Color.white;
            players[1].GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(0.15f);
        }

        yield return null;
    }

    private IEnumerator ScreenAnimation(Vector3 spawnPoint)
    {
        //Open Battle Screen
        finishBattleScreenAnimation = false;
        canvas_screen.SetActive(true);
        Transform leftSide = canvas_screen.transform.Find("Left_Side");
        Transform rightSide = canvas_screen.transform.Find("Right_Side");
        Transform middleImage = canvas_screen.transform.Find("Middle_Image");

        float originalLeftPositionX = leftSide.position.x;

        Debug.Log("Start animation");

        float speed = 1500.0f;
        float speedTransparent = 0.03f;
        float rotation = 0;

        // Move Sides to center
        while (leftSide.position.x < 480)
        {
            leftSide.position += new Vector3(speed * Time.deltaTime, 0, 0);
            rightSide.position -= new Vector3(speed * Time.deltaTime, 0, 0);
            yield return null;
        }

        //Appear battle logo and change scene
        RawImage image = middleImage.gameObject.GetComponent<RawImage>();
        var tempColor = image.color;
        while (tempColor.a < 1) //appear logo
        {
            tempColor.a += speedTransparent;
            image.color = tempColor;
            if (rotation < 45)
                middleImage.transform.Rotate(0, 0, rotation += 0.5f);
            yield return null;
        }

        //put enemies back to initial position
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyMovement>().ReturnToInitialPosition();
        }

        yield return new WaitForSeconds(0.5f);

        //Move player to battle zone
        players[0].transform.position = spawnPoint;
        players[0].GetComponent<PlayerMovement>().StopPlayer(true);
        players[1].transform.position = spawnPoint;
        players[1].GetComponent<PlayerMovement>().StopPlayer(true);

        yield return new WaitForSeconds(0.5f);

        while (tempColor.a > 0) //Disappear logo
        {
            tempColor.a -= speedTransparent;
            image.color = tempColor;
            yield return null;
        }

        // Move Sides to edges
        while (leftSide.position.x > originalLeftPositionX)
        {
            leftSide.position -= new Vector3(speed * Time.deltaTime, 0, 0);
            rightSide.position += new Vector3(speed * Time.deltaTime, 0, 0);
            yield return null;
        }

        yield return null;
    }


    public void changeLives(bool addLive)
    {
        if (addLive && lives < maxLives) //add live
        {
            Transform[] childrenLives = background_lives.GetComponentsInChildren<Transform>(true);
            GameObject lastLive = childrenLives[childrenLives.Length - 1].gameObject;
            Instantiate(livePrefab, new Vector3(lastLive.transform.position.x + 100, background_lives.transform.position.y, background_lives.transform.position.z), Quaternion.identity, background_lives.transform);
            lives++;
        }
        else if (!addLive && lives > 0) //remove live
        {
            Transform[] childrenLives = background_lives.GetComponentsInChildren<Transform>(true);
            GameObject lastLive = childrenLives[childrenLives.Length - 1].gameObject;
            Destroy(lastLive);
            lives--;

            if (lives == 0)
            {
                Debug.Log("Player Death");
                StartCoroutine(playerDeath());
            }
        }
    }

    private IEnumerator playerDeath()
    {
        //Stop player movement and camera
        players[0].GetComponent<PlayerMovement>().StopPlayer(true);
        players[1].GetComponent<PlayerMovement>().StopPlayer(true);
        finishBattleScreenAnimation = false;

        //Enable canvas battlezone to show defeat message
        canvas_battlezone.SetActive(true);
        canvas_battlezone.transform.Find("OptionsButton").gameObject.SetActive(false);

        TextMeshProUGUI victoryText = canvas_battlezone.transform.Find("VictoryText").GetComponent<TextMeshProUGUI>();
        victoryText.text = "YOU DIED!";

        while (victoryText.fontSize < 300) //appear logo
        {
            victoryText.fontSize += 0.25f;
            yield return null;
        }

        //disable text and battle screen
        victoryText.text = "";
        victoryText.fontSize = 150;
        canvas_battlezone.SetActive(false);
        canvas_battlezone.transform.Find("OptionsButton").gameObject.SetActive(true);

        //finish battle animation
        yield return StartCoroutine(ScreenAnimation_Home(currentRoomPosition.transform.position));
        finishBattleScreenAnimation = true;

        yield return null;
    }

    public void BattleZone_Fight()
    {
        //initiate combat
        Debug.Log("BattleZone - Fight");
        players[0].GetComponent<PlayerMovement>().StopPlayer(false);
        players[1].GetComponent<PlayerMovement>().StopPlayer(false);
        canvas_battlezone.SetActive(false);
    }



    public void BattleZone_Heal()
    {
        Debug.Log("BattleZone - Heal");
        changeLives(true);
        players[0].GetComponent<PlayerMovement>().StopPlayer(false);
        players[1].GetComponent<PlayerMovement>().StopPlayer(false);
        canvas_battlezone.SetActive(false);
    }

    public void BattleZone_Dialogue()
    {
        Debug.Log("BattleZone - Dialogue");
    }

    public void BattleZone_Escape()
    {
        canvas_battlezone.SetActive(false);
        StartCoroutine(ScreenAnimation_Home(currentRoomPosition.transform.position));
        Debug.Log("BattleZone - Escape");
    }

}
