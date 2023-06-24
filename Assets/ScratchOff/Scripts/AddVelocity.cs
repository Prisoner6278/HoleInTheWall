using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddVelocity : MonoBehaviour
{
    //  This script adds velocity to ball

    //      VARIABLES

    [SerializeField] private float forceAmount;
    [SerializeField] private Vector3 direction;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        Rigidbody otherRB = other.gameObject.GetComponent<Rigidbody>();
        otherRB.AddForce(direction.normalized * forceAmount, ForceMode.Impulse);
    }

}
