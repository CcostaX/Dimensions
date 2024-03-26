using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    public float moveSpeed = 0.11f;
    private Rigidbody2D rb2D;
    private Rigidbody rb;
    [SerializeField] private GameObject[] players = new GameObject[2];
    private SpriteRenderer spriteRenderer;
    public bool isBattleMode = true;
    public bool isDangerous = false;
    private Vector3 moveDirection;

    public GameObject spawnPoint;

    public float slimeMovementInterval = 10f;
    private float slimeTimer = 0f;

    private float timer = 0f;

    [SerializeField] private CameraView cameraView;
    [SerializeField] private GameObject canvas_battlezone;
    public GameObject[] walls;

    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
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
        // Generate a random direction
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);

        // Normalize the direction to ensure constant speed
        moveDirection = new Vector2(randomX, randomY);

        initialPosition = transform.position;

        cameraView = GameObject.Find("MainCamera").GetComponent<CameraView>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (name.StartsWith("Slime"))
        {
            SlimeMovement();
        }
    }

    private void SlimeMovement()
    {
        float distance2D = Vector2.Distance(players[0].transform.position, this.transform.position); //battle zone distance 2D
        float distance3D = Vector2.Distance(players[1].transform.position, this.transform.position); //battle zone distance 3D
        if (isBattleMode)
        {
            if ((distance2D < 10 || distance3D < 10) && (!canvas_battlezone.activeSelf && gameManager.finishBattleScreenAnimation))
            {
                if (slimeTimer < slimeMovementInterval)
                {
                    slimeTimer += Time.fixedDeltaTime;
                    if (cameraView.isDimension2D)
                    {
                        //2D movement
                        rb2D = GetComponent<Rigidbody2D>();
                        if (rb2D != null)
                        {
                            Vector2 normalizedDirection = moveDirection.normalized;
                            transform.Translate(normalizedDirection.x * moveSpeed * Time.deltaTime, normalizedDirection.y * moveSpeed * Time.deltaTime, 0);
                        }
                    }
                    else
                    {
                        //3D movement
                        rb = GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            Vector3 normalizedDirection = moveDirection.normalized;
                            transform.Translate(normalizedDirection.x * moveSpeed * Time.deltaTime, normalizedDirection.y * moveSpeed * Time.deltaTime, 0);
                        }
                    }
                }
                else
                {
                    StartCoroutine(BackToPosition());
                }
            }
        }
        else
        {
            if (cameraView.isDimension2D)
            {
                if (distance2D < 7.5)
                {
                    Vector2 pos = Vector2.MoveTowards(transform.position, players[0].transform.position, moveSpeed * Time.deltaTime);
                    rb2D = GetComponent<Rigidbody2D>();
                    if (rb2D != null)
                    {
                        rb2D.MovePosition(pos);
                        spriteRenderer = GetComponent<SpriteRenderer>();
                        if (players[0].transform.position.x < this.transform.position.x)
                            spriteRenderer.flipX = true;
                        else
                            spriteRenderer.flipX = false;
                    }
                }
            }
            else
            {
                if (distance3D < 7.5)
                {
                    Vector3 pos = Vector3.MoveTowards(transform.position, players[1].transform.position, moveSpeed * Time.deltaTime);
                    rb = GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.MovePosition(pos);
                        spriteRenderer = GetComponent<SpriteRenderer>();
                        if (players[1].transform.position.x < this.transform.position.x)
                            spriteRenderer.flipX = true;
                        else
                            spriteRenderer.flipX = false;
                    }                   
                }
            }
        }
    }

    IEnumerator BackToPosition()
    {
        if (cameraView.isDimension2D)
            transform.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        else
            transform.gameObject.GetComponent<BoxCollider>().enabled = false;

        float distance = Vector3.Distance(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(initialPosition.x, initialPosition.y, 0));
        while (distance > 0.1f)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                new Vector3(initialPosition.x, initialPosition.y, initialPosition.z), 
                moveSpeed * Time.deltaTime
            );
            yield return new WaitForSeconds(0.1f);
            distance = Vector3.Distance(new Vector3(transform.position.x, transform.position.y, 0), new Vector3(initialPosition.x, initialPosition.y, 0));
        }

        if (cameraView.isDimension2D)
            transform.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        else
            transform.gameObject.GetComponent<BoxCollider>().enabled = true;

        canvas_battlezone.SetActive(true);
        players[0].GetComponent<PlayerMovement>().StopPlayer(true);
        players[1].GetComponent<PlayerMovement>().StopPlayer(true);
        slimeTimer = 0f;
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && isBattleMode && timer > 0.1f)
        {
            foreach (GameObject wall in walls)
            {
                if (collision.gameObject.name == wall.name && (wall.name.Contains("up") || wall.name.Contains("down")))
                {
                    ChangeEnemyDirection(true);
                }
                else if (collision.gameObject.name == wall.name && (wall.name.Contains("left") || wall.name.Contains("right")))
                {
                    ChangeEnemyDirection(false);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall" && isBattleMode)
        {
            foreach (GameObject wall in walls)
            {
                if (collision.gameObject.transform.parent.name == wall.name && (wall.name.Contains("up") || wall.name.Contains("down")))
                {
                    ChangeEnemyDirection(true);
                    return;
                }
                else if (collision.gameObject.transform.parent.name == wall.name && (wall.name.Contains("left") || wall.name.Contains("right")))
                {
                    ChangeEnemyDirection(false);
                }
            }
        }
    }

    private void ChangeEnemyDirection(bool changeX)
    {
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
 
        if (changeX)
            moveDirection = new Vector2(randomX, -moveDirection.y);
        else
            moveDirection = new Vector2(-moveDirection.x, randomY);
        timer = 0f;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (moveDirection.x < 0f)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        return;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.tag == "Player")
        {
            GameObject player = collision.transform.parent.gameObject;
            if (isBattleMode) //Remove 1 life in battle
                StartCoroutine(player.GetComponent<PlayerMovement>().PlayerHit(player));
            else
            {
                if (isDangerous) //Battle Mode
                {
                    //Spawn Enemy
                    GameObject enemies = GameObject.Find("BattleZone/Enemies").gameObject;
                    enemies.transform.Find("Slime").gameObject.SetActive(true);
                    //Spawn in the battle zone room
                    gameManager.currentEnemyHit = this.gameObject;
                    StartCoroutine(gameManager.ScreenAnimation_BattleZone(spawnPoint.transform.position));
                }
                else
                {
                    //Spawn in the beggining of the room
                    collision.transform.parent.gameObject.transform.position = player.GetComponent<PlayerMovement>().currentSpawnPoint.transform.position;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.parent.tag == "Player")
        {
            GameObject player = collision.transform.parent.gameObject;
            if (isBattleMode) //Remove 1 life in battle
                StartCoroutine(player.GetComponent<PlayerMovement>().PlayerHit(player));
            else
            {
                if (isDangerous) //Battle Mode
                {
                    //Spawn Enemy
                    GameObject enemies = GameObject.Find("BattleZone/Enemies").gameObject;
                    enemies.transform.Find("Slime").gameObject.SetActive(true);
                    //Spawn in the battle zone room
                    gameManager.currentEnemyHit = this.gameObject;
                    StartCoroutine(gameManager.ScreenAnimation_BattleZone(spawnPoint.transform.position));
                }
                else
                {
                    //Spawn in the beggining of the room
                    collision.transform.parent.gameObject.transform.position = player.GetComponent<PlayerMovement>().currentSpawnPoint.transform.position;
                }
            }
        }
    }

    public void ReturnToInitialPosition()
    {
        transform.position = initialPosition;
    }
}
