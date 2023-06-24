using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackHand : MonoBehaviour
{
    private List<StackItem> stack = new List<StackItem>();
    [SerializeField] private BoxCollider stackHitbox;

    [SerializeField] private Transform lHand;

    private float stackHitboxDefaultSize;
    private float stackHitboxDefaultCenter;

    private void Awake()
    {
        stackHitboxDefaultSize = stackHitbox.size.y;
        stackHitboxDefaultCenter = stackHitbox.center.y;
    }

    public void AddToStack(BucketItem item)
    {
        item.GetComponentInChildren<SpriteRenderer>().sortingOrder = stack.Count;

        StackItem stackItem;
        if(stack.Count > 0)
        {
            int prevIndex = stack.Count - 1;
            stackItem = new StackItem(item, stack[prevIndex].pos + new Vector3(0, 1.3f, 0));

            stackHitbox.size = new Vector3(stackHitbox.size.x, stackHitbox.size.y + .288f, stackHitbox.size.z);
            stackHitbox.center = new Vector3(stackHitbox.center.x, stackHitbox.center.y + .144f, stackHitbox.center.z);
        }
        else
        {
            stackItem = new StackItem(item, Vector3.zero);

            stackHitbox.size = new Vector3(stackHitbox.size.x, stackHitbox.size.y + .288f, stackHitbox.size.z);
            stackHitbox.center = new Vector3(stackHitbox.center.x, stackHitbox.center.y + .144f, stackHitbox.center.z);
        }
        stack.Add(stackItem);
    }

    public void ClearStack()
    {
        stack.Clear();

        stackHitbox.size = new Vector3(stackHitbox.size.x, stackHitboxDefaultSize, stackHitbox.size.z);
        stackHitbox.center = new Vector3(stackHitbox.center.x, stackHitboxDefaultCenter, stackHitbox.center.z);
    }

    public void StackFallOver(int direction)
    {
        foreach (StackItem stackItem in stack)
        {
            stackItem.item.StartSlap(direction);
        }

        ClearStack();
    }

    public List<BucketItem> GetBucketItems()
    {
        List<BucketItem> bucketItems = new List<BucketItem> ();
        foreach (StackItem stackItem in stack)
        {
            bucketItems.Add(stackItem.item);
        }

        return bucketItems;
    }

    private void Update()
    {
        foreach(StackItem stackItem in stack)
        {
            stackItem.item.gameObject.transform.position = stackItem.pos + lHand.position;
        }
    }

    public class StackItem
    {
        public BucketItem item;
        public Vector3 pos;

        public StackItem(BucketItem item)
        {
            this.item = item;
        }

        public StackItem(BucketItem item, Vector3 pos)
        {
            this.item = item;
            this.pos = pos;
        }

        public void SetPosition(Vector3 pos)
        {
            this.pos = pos;
        }
    }
}
