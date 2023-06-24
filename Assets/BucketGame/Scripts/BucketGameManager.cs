using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.Events;

public class BucketGameManager : MonoBehaviour
{
    [SerializeField] private SpawnBucketItems[] spawners;
    [SerializeField] private float gameTimerSeconds;
    private float initialTimer;
    [SerializeField] private Bucket bucket;
    [SerializeField] private TrashBucket trashBucket;
    [SerializeField] private TextMeshProUGUI timerTextBox;
    [SerializeField] private TextMeshProUGUI collectedTextBox;
    [SerializeField] private GameObject gameOverUI;
    private bool ended = false;

    [SerializeField] private StackHand LHand;

    [SerializeField] private SoundCue musicCue;

    [SerializeField] private float maxSwitchDelta;
    [SerializeField] private TimerEvents[] timerEvents;
    [SerializeField] private ScoreEvents[] scoreEvents;
    private bool swapActive;
    Vector3 oldTrashLocation;
    Vector3 oldBucketLocation;
    Vector3 originalTrashLocation;
    Vector3 originalBucketLocation;


    private void Start()
    {
        initialTimer = gameTimerSeconds;
        gameOverUI.SetActive(false);
        originalBucketLocation = bucket.gameObject.transform.position;
        originalTrashLocation = trashBucket.gameObject.transform.position;

        musicCue.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (ended)
            return;

        gameTimerSeconds -= Time.deltaTime;
        DisplayTimer();


        foreach (TimerEvents timerEvent in timerEvents)
        {
            if (timerEvent.activated)
                continue;

            if (gameTimerSeconds <= timerEvent.ActiveTime)
            {
                timerEvent.activated = true;
                timerEvent.InvokeEvent.Invoke();
            }
        }
        foreach (ScoreEvents scoreEvent in scoreEvents)
        {
            if (scoreEvent.activated)
                continue;

            if (bucket.Score >= scoreEvent.ActivePoints)
            {
                scoreEvent.activated = true;
                scoreEvent.InvokeEvent.Invoke();
            }
        }


        if (swapActive)
        {
            SwapBuckets();
        }

        if(gameTimerSeconds <= 0)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        ended = true;
        gameTimerSeconds = 0;
        DisplayTimer();

        SpawnersSetActive(false);
        GameObject[] bucketItems = GameObject.FindGameObjectsWithTag("BucketItem");
        foreach (GameObject item in bucketItems)
        {
            Destroy(item);
        }
        LHand.ClearStack();

        musicCue.Stop();

        gameOverUI.SetActive(true);
        collectedTextBox.text = $"Good Food Collected: {bucket.GoodScore}\nBad Items Swatted: {bucket.BadScore}\nTotal Score: {bucket.Score}";
    }

    public void Replay()
    {
        gameTimerSeconds = initialTimer;
        gameOverUI.SetActive(false);
        bucket.ResetScore();

        swapActive = false;
        bucket.gameObject.transform.position = originalBucketLocation;
        trashBucket.gameObject.transform.position = originalTrashLocation;

        musicCue.Play();

        ended = false;
        SpawnersSetActive(true);
    }

    private void DisplayTimer()
    {
        int minute = (int)gameTimerSeconds / 60;
        int seconds = (int)(gameTimerSeconds - minute * 60);

        int trunc = (int)gameTimerSeconds;
        int miliseconds = (int)((gameTimerSeconds - (float)trunc) * 100f);

        string minuteString = minute.ToString("D2");
        string secondsString = seconds.ToString("D2");
        string mSecondsString = miliseconds.ToString("D2");
        timerTextBox.text = $"{minuteString}:{secondsString}.{mSecondsString}";
    }

    private void SpawnersSetActive(bool isActive)
    {
        foreach (SpawnBucketItems spawn in spawners)
        {
            spawn.gameObject.SetActive(isActive);
        }
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    public void StartSwapBuckets()
    {
        if (swapActive)
            return;

        oldTrashLocation = trashBucket.gameObject.transform.position;
        oldBucketLocation = bucket.gameObject.transform.position;
        swapActive = true;
    }

    private void SwapBuckets()
    {
        bucket.transform.position = Vector3.MoveTowards(bucket.transform.position, oldTrashLocation, maxSwitchDelta * Time.deltaTime);
        trashBucket.transform.position = Vector3.MoveTowards(trashBucket.transform.position, oldBucketLocation, maxSwitchDelta * Time.deltaTime);

        if(bucket.transform.position == oldTrashLocation && trashBucket.transform.position == oldBucketLocation)
        {
            swapActive = false;
        }
    }

    public void ReduceAllTimesToSpawn()
    {
        foreach (SpawnBucketItems spawner in spawners)
        {
            spawner.ReduceTimeToSpawn();
        }
    }
}

[Serializable]
public class TimerEvents
{
    [field: SerializeField] public float ActiveTime { get; private set; }
    [field: SerializeField] public UnityEvent InvokeEvent { get; private set; }
    [HideInInspector] public bool activated = false;
}

[Serializable]
public class ScoreEvents
{
    [field: SerializeField] public int ActivePoints { get; private set; }
    [field: SerializeField] public UnityEvent InvokeEvent { get; private set; }
    [HideInInspector] public bool activated = false;
}
