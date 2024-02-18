using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private float meleeSpeed;
    [SerializeField] private float damage;
    [SerializeField] private GameObject sword;
    [SerializeField] private GameManager gameManager;

    private float timeUntilMelee;
    private float hitCooldownTimer = 0f;

    private void Start()
    {
        anim = GetComponent<Animator>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.currentRoom >= 0)
        {
            return;
        }

        if (timeUntilMelee <= 0)
        {
            if (Input.GetKeyDown(KeyCode.C)) 
            {
                anim.SetTrigger("attack");
                timeUntilMelee = meleeSpeed;
            }
        }
        else
        {
            timeUntilMelee -= Time.deltaTime;
        }

        hitCooldownTimer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && hitCooldownTimer >= 0.5f)
        {
            Debug.Log("Enemy hit");
            hitCooldownTimer = 0f;
            int enemyLife = collision.gameObject.GetComponent<Enemy>().enemyLive;
            if (enemyLife > 0)
                collision.gameObject.GetComponent<Enemy>().enemyLive--;
            else
                Destroy(collision.gameObject);

        }
    }
}
