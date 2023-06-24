using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBucket : MonoBehaviour
{
    [SerializeField] private Bucket bucket;

    [SerializeField] private SoundCue soundCue;
    [SerializeField] private Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Hand"))
        {
            Player_Catch hand = other.GetComponent<Player_Catch>();
            bool handWasEmpty = true;
            foreach (BucketItem item in hand.HeldObjects)
            {
                if (item.ItemType == BucketItem.BUCKET_ITEM_TYPE.BAD)
                {
                    bucket.AddScoreFromGarbage();
                }

                Destroy(item.gameObject);
                handWasEmpty = false;
            }

            hand.ClearHand();

            if(!handWasEmpty)
            {
                soundCue.PlayOneShot();
                animator.Play("Trash Bucket Bounce");
            }
        }
    }
}
