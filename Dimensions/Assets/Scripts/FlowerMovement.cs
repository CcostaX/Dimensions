using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlowerMovement : MonoBehaviour
{
    public bool isBlooming;
    public bool isLeft;
    public bool isUp;
    public bool isHorizontal;
    public GameObject flower;
    public float flowerTimer;
    float spawnTime = 0f;
    private float movementDuration = 5f;
    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Random.Range(1, 6); // 1 to 5
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBlooming)
        {
            return;
        }

        flowerTimer += Time.deltaTime;
        if (flowerTimer >= spawnTime)
        {
            float randomX = transform.position.x;
            float randomY = transform.position.y;

            if (isHorizontal)
            {
                if (isUp)
                    randomY = Random.Range(transform.position.y - 1f, transform.position.y + 6f);
                else
                    randomY = Random.Range(transform.position.y - 6f, transform.position.y + 1f);
            }
            else
            {
                if (isLeft)
                    randomX = Random.Range(transform.position.x - 1f, transform.position.x + 5f);
                else
                    randomX = Random.Range(transform.position.x - 5f, transform.position.x + 1f);
            }


            //new flower
            GameObject newFlower = Instantiate(
                flower,
                new Vector3(transform.position.x, transform.position.y, transform.position.z - 1),
                transform.rotation
            );
            newFlower.transform.parent = transform;

            StartCoroutine(MoveFlowerCoroutine(newFlower.transform, new Vector3(randomX, randomY, transform.position.z)));

            Destroy(newFlower, 20f); //destroy after 5 seconds
            spawnTime = Random.Range(1, 6);
            flowerTimer = 0f;
        }
    }

    private IEnumerator MoveFlowerCoroutine(Transform flowerTransform, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = flowerTransform.position;

        while (elapsedTime < movementDuration)
        {
            // Calculate the interpolation ratio based on elapsed time and movement duration
            float t = elapsedTime / movementDuration;

            // Move the flower towards the target position gradually
            flowerTransform.position = Vector3.Lerp(startingPosition, targetPosition, t);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the flower ends up exactly at the target position
        flowerTransform.position = targetPosition;
    }
}
