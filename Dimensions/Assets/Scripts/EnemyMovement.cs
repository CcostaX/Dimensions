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
    private bool isBattleMode = true;
    private Vector3 moveDirection;

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
            if (cameraView.isDimension2D)
            {
                //2D movement
                rb2D = GetComponent<Rigidbody2D>();
                if (rb2D != null)
                    rb2D.velocity = moveDirection * moveSpeed;
            }
            else
            {
                //3D movement
                rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Debug.Log(moveDirection);
                    rb.velocity = moveDirection * moveSpeed;
                }
            }
        }
        else
        {
            float distance = Vector3.Distance(player.transform.position, this.transform.position);
            if (distance < 7.5)
            {
                Vector3 pos = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
                rb2D.MovePosition(pos);
                transform.LookAt(player.transform);
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (player.transform.position.x < this.transform.position.x)
                    spriteRenderer.flipX = true;
                else
                    spriteRenderer.flipX = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && isBattleMode && timer > 0.1f)
        {
            float randomX = Random.Range(-1f, 1f);
            float randomY = Random.Range(-1f, 1f);
            foreach (GameObject wall in walls)
            {
                if (collision.gameObject.name == wall.name && (wall.name.Contains("up") || wall.name.Contains("down")))
                {
                    moveDirection = new Vector2(randomX, -moveDirection.y);
                    timer = 0f;
                    return;
                }
                else if (collision.gameObject.name == wall.name && (wall.name.Contains("left") || wall.name.Contains("right")))
                {
                    moveDirection = new Vector2(-moveDirection.x, randomY);
                    timer = 0f;
                    return;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall" && isBattleMode)
        {
            float randomX = Random.Range(-1f, 1f);
            float randomY = Random.Range(-1f, 1f);
            foreach (GameObject wall in walls)
            {
                if (collision.gameObject.transform.parent.name == wall.name && (wall.name.Contains("up") || wall.name.Contains("down")))
                {
                    moveDirection = new Vector2(randomX, -moveDirection.y);
                    return;
                }
                else if (collision.gameObject.transform.parent.name == wall.name && (wall.name.Contains("left") || wall.name.Contains("right")))
                {
                    moveDirection = new Vector2(-moveDirection.x, randomY);
                    return;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.tag == "Player")
        {
            GameObject player = collision.transform.parent.gameObject;
            StartCoroutine(player.GetComponent<PlayerMovement>().PlayerHit(player));
        }
    }
}
