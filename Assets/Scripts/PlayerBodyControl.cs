using System;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity.CoreModule;
using OpenCVForUnityExample;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBodyControl : MonoBehaviour
{
    public WebcamPoints webCamPoints;
    public GameObject playArea;

    public GameObject RWristObj;
    public GameObject LWristObj;
    //public GameObject rForeArm;
    //public GameObject rUpperArm;
    //public GameObject lForeArm;
    //public GameObject lUpperArm;
    [SerializeField] private float handDistanceFactor = 1f;
    [field: SerializeField] public GameObject LHandObj { get; private set; }
    [field:SerializeField] public GameObject RHandObj { get; private set; }

    [field:SerializeField] public GameObject Torso { get; private set; }

    public float convesionScale = 0.06f;

    [SerializeField] private Vector3 bodyOffset;

    Point rWrist;
    Point lWrist;

    Point rElbow;
    Point lElbow;

    Point torso;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rWrist = webCamPoints.BodyPartData["RWrist"];
        lWrist = webCamPoints.BodyPartData["LWrist"];

        rElbow = webCamPoints.BodyPartData["RElbow"];
        lElbow = webCamPoints.BodyPartData["LElbow"];

        torso = webCamPoints.BodyPartData["Nose"];

        if (!(Mathf.Approximately((float) rWrist.x, (float) lWrist.x) ||
              (Mathf.Approximately((float) rWrist.x, (float) lWrist.x))))
        {
            //Wrist webcam to world
            Vector3 rWrist_position = new Vector3((float) rWrist.x, (float) -rWrist.y, 0.0f);
            rWrist_position += new Vector3(-webCamPoints.transform.localScale.x / 2,
                webCamPoints.transform.localScale.y / 2, 0);
            rWrist_position *= convesionScale;

            Vector3 lWrist_position = new Vector3((float) lWrist.x, (float) -lWrist.y, 0.0f);
            lWrist_position += new Vector3(-webCamPoints.transform.localScale.x / 2,
                webCamPoints.transform.localScale.y / 2, 0);
            lWrist_position *= convesionScale;

            //Elbow webcam to world
            Vector3 rElbow_position = new Vector3((float)rElbow.x, (float)-rElbow.y, 0.0f);
            rElbow_position += new Vector3(-webCamPoints.transform.localScale.x / 2,
                webCamPoints.transform.localScale.y / 2, 0);
            rElbow_position *= convesionScale;

            Vector3 lElbow_position = new Vector3((float)lElbow.x, (float)-lElbow.y, 0.0f);
            lElbow_position += new Vector3(-webCamPoints.transform.localScale.x / 2,
                webCamPoints.transform.localScale.y / 2, 0);
            lElbow_position *= convesionScale;

            //Torso webcam to world
            Vector3 torso_position = new Vector3((float)torso.x, (float)-torso.y, 0.0f);
            torso_position += new Vector3(-webCamPoints.transform.localScale.x / 2,
                webCamPoints.transform.localScale.y / 2, 0);
            torso_position *= convesionScale;

            Vector3 lHand_position = FindHandPosition(lElbow_position, lWrist_position);
            Vector3 rHand_position = FindHandPosition(rElbow_position, rWrist_position);

            RWristObj.transform.position = rWrist_position + bodyOffset;
            LWristObj.transform.position = lWrist_position + bodyOffset;

            RHandObj.transform.position = rHand_position + bodyOffset;
            LHandObj.transform.position = lHand_position + bodyOffset;

            Torso.transform.position = torso_position + bodyOffset;
        }

        /*
        if ( !(Mathf.Approximately((float)rWrist.x, (float)lWrist.x) || (Mathf.Approximately((float)rWrist.x, (float)lWrist.x))))
        {
            UpdateBodyPart("RWrist", "RElbow", rForeArm, false);
            UpdateBodyPart("LWrist", "LElbow", lForeArm,false);
        }
        
        UpdateBodyPart("RElbow", "RShoulder", rUpperArm, false);
        UpdateBodyPart("LElbow", "LShoulder", lUpperArm, false);
        */
    }

    //Estimate the hand position from the elbow and wrist
    private Vector3 FindHandPosition(Vector3 elbow, Vector3 wrist)
    {
        Vector3 direction = (wrist - elbow).normalized;
        return wrist + (direction * handDistanceFactor);
    }

    // Dont need this for the time being just going with giant collider at wrist
    void UpdateBodyPart(string startPart, string endPart, GameObject physicsBody, bool forceLoc)
    {
        Point startPoint = webCamPoints.BodyPartData[startPart];
        Point endPoint = webCamPoints.BodyPartData[endPart];

        if (startPoint.x < 0 || endPoint.x < 0)
        {
            return;
        }
        
        Vector3 startPart_position = new Vector3((float) startPoint.x, (float) -startPoint.y, 0.0f);
        startPart_position += new Vector3(-webCamPoints.transform.localScale.x / 2, webCamPoints.transform.localScale.y / 2, 0);
        startPart_position *= convesionScale;
        
        Vector3 endPart_position = new Vector3((float) endPoint.x, (float) -endPoint.y, 0.0f);
        endPart_position += new Vector3(-webCamPoints.transform.localScale.x / 2, webCamPoints.transform.localScale.y / 2, 0);
        endPart_position *= convesionScale;

        Vector3 direction = endPart_position - startPart_position;
        

      // if (forceLoc == false)
      // {
           physicsBody.transform.position = Vector3.MoveTowards(physicsBody.transform.position,
               (startPart_position + (direction * 0.5f)), 20.0f * Time.deltaTime);
      // }
       //else
      // {
      //     physicsBody.transform.position = startPart_position + (direction * 0.5f);
       //}
       
       //Vector3 targetDirection =
       //    Vector3.RotateTowards(physicsBody.transform.up, direction.normalized, 10.0f * Time.deltaTime, 0.0f);
       Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);
       physicsBody.transform.rotation = rotation;
       
       
       physicsBody.transform.localScale = new Vector3(1f, 
           Mathf.Lerp(physicsBody.transform.localScale.y, direction.magnitude * 0.5f, 0.5f ) , 1f);
    }
}
