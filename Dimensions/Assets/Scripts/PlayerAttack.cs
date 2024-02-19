using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private float meleeSpeed;
    [SerializeField] private float damage;
    [SerializeField] private GameObject sword;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject canvas_battlezone;

    private float timeUntilMelee;
    public float hitCooldownTimer = 0f;

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

    public IEnumerator VictoryScreen(GameObject currentEnemyInBattleZone)
    {
        GameObject victoryText = canvas_battlezone.transform.Find("VictoryText").gameObject;

        //show battle zone and victory text
        canvas_battlezone.SetActive(true);
        canvas_battlezone.transform.Find("OptionsButton").gameObject.SetActive(false);
        Destroy(currentEnemyInBattleZone);
        victoryText.GetComponent<TextMeshProUGUI>().text = "Victory";

        yield return new WaitForSeconds(3f);

        //hide battle zone and victory text
        victoryText.GetComponent<TextMeshProUGUI>().text = "";
        canvas_battlezone.transform.Find("OptionsButton").gameObject.SetActive(true);
        canvas_battlezone.SetActive(false);

        //destroy enemy on puzzle
        Destroy(gameManager.currentEnemyHit);
        gameManager.currentEnemyHit = null;

        //put player and other enemies back to initial position
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyMovement>().ReturnToInitialPosition();
        }

        StartCoroutine(gameManager.ScreenAnimation_Home(gameManager.originalPlayerPosition));

        yield return null;
    }

    public void Victory(GameObject currentEnemyInBattleZone)
    {
        StartCoroutine(VictoryScreen(currentEnemyInBattleZone));
        //transform.position = gameManager.originalPlayerPosition;
    }
}
