using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyLive = 1;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject canvas_battlezone;
    [SerializeField] private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerAttack")
        {
            GameObject player = collision.gameObject.transform.parent.parent.parent.gameObject;
            if (player.GetComponent<PlayerAttack>().hitCooldownTimer >= 0.5f)
            {
                player.GetComponent<PlayerAttack>().hitCooldownTimer = 0f;
                if (enemyLive > 0)
                    StartCoroutine(EnemyHit(this.gameObject));
                else
                    player.GetComponent<PlayerAttack>().Victory(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "PlayerAttack")
        {
            if (collision.tag == "PlayerAttack")
            {
                GameObject player = collision.gameObject.transform.parent.parent.parent.gameObject;
                if (player.GetComponent<PlayerAttack>().hitCooldownTimer >= 0.5f)
                {
                    player.GetComponent<PlayerAttack>().hitCooldownTimer = 0f;
                    if (enemyLive > 0)
                        StartCoroutine(EnemyHit(this.gameObject));
                    else
                        player.GetComponent<PlayerAttack>().Victory(this.gameObject);
                }
            }
        }
    }

    IEnumerator EnemyHit(GameObject enemy)
    {
        for (int i = 0; i < 3; i++)
        {
            enemy.GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.15f);
            enemy.GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(0.15f);
        }
        enemyLive--;
        yield return null;
    }
}
