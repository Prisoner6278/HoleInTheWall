using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BucketItem : MonoBehaviour
{
    public enum BUCKET_ITEM_TYPE
    {
        GOOD = 0,
        BAD = 1
    }

    [field: SerializeField] public BUCKET_ITEM_TYPE ItemType { get; private set; }
    [SerializeField] private float gravityScale = 1;
    private const float gravity = -9.81f;

    [SerializeField] private SoundCue soundCue;

    public bool IsGrabbed { get; private set; }
    public bool IsSlapped { get; private set; }
    private int slapDirection = 1;

    private void Update()
    {
        if(transform.position.y < -20)
        {
            Destroy(gameObject);
        }

        if (IsGrabbed)
            return;

        if (!IsSlapped)
            ApplyGravity();

        if (IsSlapped)
            ApplySlap();
    }

    private void ApplyGravity()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + gravity * gravityScale * Time.deltaTime, transform.position.z);
    }

    public void ApplySlap()
    {
        float randomRotate = 0;
        randomRotate = Random.Range(25, 35);
        randomRotate *= slapDirection;

        float randomPositionDelta = 0;
        randomPositionDelta = Random.Range(6, 12);
        randomPositionDelta *= slapDirection;

        transform.position = new Vector3(transform.position.x + randomPositionDelta * Time.deltaTime, transform.position.y + gravity * gravityScale * Time.deltaTime, transform.position.z);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + randomRotate);
    }

    public void StartSlap(int direction)
    {
        IsGrabbed = false;
        IsSlapped = true;
        slapDirection = direction;
    }

    public void GrabObject()
    {
        IsGrabbed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsGrabbed || IsSlapped)
            return;

        if(other.CompareTag("LHand"))
        {
            StackHand stack = other.GetComponent<StackHand>();

            if (ItemType == BUCKET_ITEM_TYPE.GOOD)
            {
                IsGrabbed = true;

                stack.AddToStack(this);
            }
            else
            {
                if (other.transform.position.x > transform.position.x)
                {
                    slapDirection = -1;
                }
                else
                {
                    slapDirection = 1;
                }

                IsSlapped = true;
                stack.StackFallOver(slapDirection * -1);
                soundCue.PlayOneShot();
            }
        }
        else if(other.CompareTag("RHand"))
        {
            if(ItemType == BUCKET_ITEM_TYPE.BAD)
            {
                IsSlapped = true;
                soundCue.PlayOneShot();

                Bucket.Instance.AddSwatScore();

                if (other.transform.position.x > transform.position.x)
                {
                    slapDirection = -1;
                }
                else
                {
                    slapDirection = 1;
                }
            }
        }
    }
}
