using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Player_Catch : MonoBehaviour
{
    public List<BucketItem> HeldObjects { get => heldObjects; }
    private List<BucketItem> heldObjects = new List<BucketItem>();
    [SerializeField] private SoundCue soundCue;

    private void FixedUpdate()
    {
        MoveItemsToHand();
    }

    private void MoveItemsToHand()
    {
        for (int i = 0; i < heldObjects.Count; i++)
        {
            BucketItem item = heldObjects[i];
            item.transform.position = gameObject.transform.position;
        }
    }

    public void ClearHand()
    {
        heldObjects.Clear();
    }

    public void AddToHand(BucketItem item)
    {
        heldObjects.Add(item);
        soundCue.PlayOneShot();
    }
}
