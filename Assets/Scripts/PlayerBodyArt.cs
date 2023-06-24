using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyArt : MonoBehaviour
{
    [Header("Torso")]
    [SerializeField] private Transform torsoGFX;
    [SerializeField] private Transform torso;
    [SerializeField] private Vector3 torsoGFXOffset;

    [Header("Left Hand")]
    [SerializeField] private Transform lHandGFX;
    [SerializeField] private Transform lHand;
    [SerializeField] private Vector3 lHandOffset;

    [Header("Right Hand")]
    [SerializeField] private Transform rHandGFX;
    [SerializeField] private Transform rHand;
    [SerializeField] private Vector3 rHandOffset;

    [Header("Lerp")]
    [SerializeField] float minError = 0.1f;
    [SerializeField] float maxError = 10.0f;

    [SerializeField] float minCorrection = 0.05f;
    [SerializeField] float maxCorrection = 0.8f;

    private void Update()
    {
        torsoGFX.position = Vector3.Lerp(torsoGFX.transform.position, torso.position + torsoGFXOffset,
            CalculateAlphaForErrorPosition(torso.position + torsoGFXOffset, torsoGFX.transform.position));


        lHandGFX.position = Vector3.Lerp(lHandGFX.transform.position, lHand.transform.position + lHandOffset,
            CalculateAlphaForErrorPosition(lHand.transform.position, lHandGFX.transform.position));

        rHandGFX.position = Vector3.Lerp(rHandGFX.transform.position, rHand.transform.position + rHandOffset, 
            CalculateAlphaForErrorPosition(rHand.transform.position, rHandGFX.transform.position));
    }

    float CalculateAlphaForErrorPosition(Vector3 realPos, Vector3 visualPosition)
    {
        float output = minError;

        float dist = Vector3.Distance(realPos, visualPosition);

        if(dist > maxError)
        {
            output = maxCorrection;
        }
        else if(dist < minError)
        {
            output = minCorrection;
        }
        else
        {
            output = dist / maxError;
        }

        return output;
    }
}
