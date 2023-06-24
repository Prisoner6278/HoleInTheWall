using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using OpenCVForUnityExample;
using OpenCVForUnity.CoreModule;

public class WebcamButton : MonoBehaviour
{
    //[SerializeField] private WebcamPoints webcamTracker;
    [SerializeField] private float distanceFromButton = 45f;
    [SerializeField] private float fillRate = 0.05f;
    [SerializeField] private float drainRate = 0.1f;
    [SerializeField] private float delayTime = 0.26f;
    [SerializeField] private PlayerBodyControl playerBodyControl;
    //[SerializeField] private bool oneTimeButton;
    [SerializeField] private UnityEvent onButtonFire;
    [SerializeField] private UnityEvent onButtonHover;
    private Image buttonFill;
    private bool running;
    private bool onHoverCooldown;

    //The delay between switching from Fill to Drain
    private float delayTimer = 0;


    private void Awake()
    {
        buttonFill = GetComponent<Image>();
    }

    private void OnEnable()
    {
        buttonFill.fillAmount = 0;
        running = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (running && oneTimeButton)
        //{
        //    return;
        //}
        if(running)
        {
            Drain();
            return;
        }

        //Debug.Log(points.RightWristScreen);
        //Debug.Log($"Button: {transform.position}, Right Wrist: {webcamTracker.RightWristScreen}");

        Vector2 LWrist = playerBodyControl.LHandObj.transform.position;
        Vector2 RWrist = playerBodyControl.RHandObj.transform.position;

        if (Vector2.Distance(transform.position, Camera.main.WorldToScreenPoint(LWrist)) <= distanceFromButton || Vector2.Distance(transform.position, Camera.main.WorldToScreenPoint(RWrist)) <= distanceFromButton)
        {
            Fill();

            delayTimer = 0;

            if(!onHoverCooldown)
            {
                onHoverCooldown = true;
                onButtonHover.Invoke();
            }
        }
        else if(delayTimer < delayTime)
        {
            SwitchDelay();
        }
        else
        {
            Drain();
            onHoverCooldown = false;
        }
    }

    private void Fill()
    {
        buttonFill.fillAmount += fillRate * Time.deltaTime;
        if(buttonFill.fillAmount >= 1)
        {
            onButtonFire.Invoke();
            buttonFill.fillAmount = 1;
            running = true;
        }
    }

    private void Drain()
    {
        buttonFill.fillAmount -= drainRate * Time.deltaTime;
        if (buttonFill.fillAmount <= 0)
        {
            buttonFill.fillAmount = 0;
            running = false;
        }
    }

    private void SwitchDelay()
    {
        delayTimer += Time.deltaTime;
    }
}
