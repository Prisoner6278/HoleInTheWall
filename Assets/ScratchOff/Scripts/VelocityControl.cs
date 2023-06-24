using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityControl : MonoBehaviour
{
    //  This script controls SFX for scratch off

    //      VARIABLES
    [SerializeField] private float maxVelocity;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //  check if object is going over max velocity
        if (rb.velocity.magnitude >= maxVelocity)
        {
            //  Use a clamp to set the vector to a max distance
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        }
        //Debug.Log(rb.velocity.magnitude);
    }
}
