using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bucket : MonoBehaviour
{
    public static Bucket Instance;
    [SerializeField] private SoundCue soundCue;
    [SerializeField] TextMeshProUGUI textBox;
    private Animator animator;
    public int Score { get; private set; }
    public int GoodScore { get; private set; }
    public int BadScore { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("Lizard Neutral");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("LHand"))
        {
            StackHand hand = other.GetComponent<StackHand>();
            bool isHappy = true;
            bool handWasEmpty = true;
            List<BucketItem> bucketItems = hand.GetBucketItems();

            foreach (BucketItem item in bucketItems)
            {
                Score++;
                GoodScore++;

                Destroy(item.gameObject);
                handWasEmpty = false;
            }
            hand.ClearStack();

            if (!handWasEmpty)
            {
                soundCue.PlayOneShot();

                if (isHappy)
                {
                    animator.Play("Lizard Happy");
                }
                else
                {
                    animator.Play("Lizard Sad");
                }
            }

            textBox.text = $"Score: {Score}";
        }
    }

    public void AddSwatScore()
    {
        BadScore++;
        Score++;

        textBox.text = $"Score: {Score}";
    }

    public void ResetScore()
    {
        Score = 0;
        GoodScore = 0;
        BadScore = 0;

        textBox.text = $"Score: {Score}";
    }

    public void AddScoreFromGarbage()
    {
        Score += 1;

        textBox.text = $"Score: {Score}";
    }
}
