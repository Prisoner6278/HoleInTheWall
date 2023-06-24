using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerps : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float maxDelta;

    [SerializeField] private Vector3 rotationTarget;
    [SerializeField] private float maxRadiansDelta;
    [SerializeField] private float maxMagnitudeDelta;

    private void Start()
    {
        
    }

    private void Update()
    {
        float singleStepPosition = maxDelta * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, singleStepPosition);

        float singleStepRotate = maxRadiansDelta * Time.deltaTime;
        float singleStepMagnitude = maxMagnitudeDelta * Time.deltaTime;
        transform.eulerAngles = Vector3.RotateTowards(transform.eulerAngles, rotationTarget, singleStepRotate, singleStepMagnitude);
    }
}
