using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBucketItems : MonoBehaviour
{
    [SerializeField] private GameObject[] goodObjects;
    private int goodObjectsLength;
    [SerializeField] private GameObject[] badObjects;
    private int badObjectsLength;

    [Header("Min Spawn Time")]
    [SerializeField] private float minSpawnTime;
    [SerializeField] private float minSpawnTimeChange;
    [SerializeField] private float minSpawnTimeClamp;

    [Header("Max Spawn Time")]
    [SerializeField] private float maxSpawnTime;
    [SerializeField] private float maxSpawnTimeChange;
    [SerializeField] private float maxSpawnTimeClamp;

    private float spawnTime;
    private float currentTimer;

    private void Start()
    {
        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        goodObjectsLength = goodObjects.Length;
        badObjectsLength = badObjects.Length;
    }

    private void Update()
    {
        currentTimer += Time.deltaTime;

        if(currentTimer >= spawnTime)
        {
            currentTimer = 0;
            spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
            int goodOrBad = Random.Range(0, 11);

            if(goodOrBad <= 1)
            {
                int random = Random.Range(0, badObjectsLength);
                GameObject spawnedObject = Instantiate(badObjects[random]);

                spawnedObject.SetActive(false);
                spawnedObject.transform.position = transform.position;
                spawnedObject.SetActive(true);
            }
            else
            {
                int random = Random.Range(0, goodObjectsLength);
                GameObject spawnedObject = Instantiate(goodObjects[random]);

                spawnedObject.SetActive(false);
                spawnedObject.transform.position = transform.position;
                spawnedObject.SetActive(true);
            }
        }
    }

    public void ReduceTimeToSpawn()
    {
        minSpawnTime = Mathf.Clamp(minSpawnTime -= Mathf.Abs(minSpawnTimeChange), minSpawnTimeClamp, Mathf.Infinity);
        maxSpawnTime = Mathf.Clamp(maxSpawnTime -= Mathf.Abs(maxSpawnTimeChange), maxSpawnTimeClamp, Mathf.Infinity);
    }
}
