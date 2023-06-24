using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratchOffRegion : MonoBehaviour
{
    [HideInInspector] public ScratchOffWin scratchOff;
    private bool isTouched = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTouched)
            return;

        if(other.CompareTag("ScratchOffBall"))
        {
            isTouched = true;
            scratchOff.ActiveRegion();
        }
    }
}
