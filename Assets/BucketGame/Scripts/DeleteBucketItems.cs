using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteBucketItems : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BucketItem"))
        {
            Destroy(other.gameObject);
        }
    }
}
