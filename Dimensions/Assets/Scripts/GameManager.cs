using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public int currentRoom = 0;
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
    public Vector3 originalPlayerPosition;
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
            //Instantiate player's lives
            Instantiate(livePrefab, new Vector3(livePrefab.transform.position.x + livePositionX, background_lives.transform.position.y, background_lives.transform.position.z), Quaternion.identity, background_lives.transform);
            livePositionX += 100;
        }
        //OTHER
        cameraView = GameObject.Find("MainCamera").GetComponent<CameraView>();
        originalPlayerPosition = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator StartScreenAnimation(GameObject battleZoneSpawnPoint)
    {
        //Obtain player position for going back
        GameObject player = GameObject.Find("Player");
        originalPlayerPosition = player.transform.position;

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
                middleImage.transform.Rotate(0, 0, rotation+=0.5f);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);     

        //Move player to battle zone
        GameObject.Find("Player").transform.position = battleZoneSpawnPoint.transform.position;
        GameObject.Find("Player").GetComponent<PlayerMovement>().StopPlayer(true);

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
        }
    }

    public void BattleZone_Fight()
    {
        //iniate combat
        Debug.Log("BattleZone - Fight");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().StopPlayer(false);
        canvas_battlezone.SetActive(false);
        //StartCoroutine(FightDuration(player, 3));
    }

    IEnumerator FightDuration(GameObject player, int duration)
    {
        yield return new WaitForSeconds(duration);
        player.GetComponent<PlayerMovement>().StopPlayer(true);
        canvas_battlezone.SetActive(true);
        yield return null;
    }


    public void BattleZone_Heal()
    {
        Debug.Log("BattleZone - Heal");
    }

    public void BattleZone_Dialogue()
    {
        Debug.Log("BattleZone - Dialogue");
    }

    public void BattleZone_Escape()
    {
        Debug.Log("BattleZone - Escape");
    }

}
