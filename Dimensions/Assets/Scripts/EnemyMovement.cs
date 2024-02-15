using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 0.11f;
    private Rigidbody2D rb2D;
    private Rigidbody rb;
    [SerializeField] private GameObject player;
    private SpriteRenderer spriteRenderer;
    public bool isBattleMode = true;
    public bool isDangerous = false;
    private Vector3 moveDirection;

    public GameObject battleZoneSpawnPoint2D;
    public GameObject battleZoneSpawnPoint;

    private float timer = 0f;

    [SerializeField] private CameraView cameraView;
    public GameObject[] walls;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        // Generate a random direction
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);

        // Normalize the direction to ensure constant speed
        moveDirection = new Vector2(randomX, randomY);

        cameraView = GameObject.Find("MainCamera").GetComponent<CameraView>();
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
        if (isBattleMode)
        {
            float distance = Vector2.Distance(player.transform.position, this.transform.position);
            if (distance < 10)
            {
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
        }
        else
        {
            if (cameraView.isDimension2D)
            {
                float distance = Vector2.Distance(player.transform.position, this.transform.position);
                if (distance < 7.5)
                {
                    Vector2 pos = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
                    rb2D = GetComponent<Rigidbody2D>();
                    if (rb2D != null)
                    {
                        rb2D.MovePosition(pos);
                        //transform.LookAt(player.transform);
                        spriteRenderer = GetComponent<SpriteRenderer>();
                        if (player.transform.position.x < this.transform.position.x)
                            spriteRenderer.flipX = true;
                        else
                            spriteRenderer.flipX = false;
                    }
                }
            }
            else
            {
                float distance = Vector3.Distance(player.transform.position, this.transform.position);
                if (distance < 7.5)
                {
                    Vector3 pos = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
                    rb = GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.MovePosition(pos);
                        //transform.LookAt(player.transform);
                        spriteRenderer = GetComponent<SpriteRenderer>();
                        if (player.transform.position.x < this.transform.position.x)
                            spriteRenderer.flipX = true;
                        else
                            spriteRenderer.flipX = false;
                    }                   
                }
            }
        }
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
        foreach (GameObject wall in walls)
        {
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.tag == "Player")
        {
            GameObject player = collision.transform.parent.gameObject;
            if (isBattleMode)
                //Remove 1 life in battle
                StartCoroutine(player.GetComponent<PlayerMovement>().PlayerHit(player));
            else
            {
                if (isDangerous)
                {
                    //Spawn in the battle zone room
                    collision.transform.parent.gameObject.transform.position = battleZoneSpawnPoint.transform.position;
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
            if (isBattleMode)
                //Remove 1 life in battle
                StartCoroutine(player.GetComponent<PlayerMovement>().PlayerHit(player));
            else
            {
                if (isDangerous)
                {
                    //Spawn in the battle zone room
                    collision.transform.parent.gameObject.transform.position = battleZoneSpawnPoint.transform.position;
                }
                else
                {
                    //Spawn in the beggining of the room
                    collision.transform.parent.gameObject.transform.position = player.GetComponent<PlayerMovement>().currentSpawnPoint.transform.position;
                }
            }
        }
    }
}
