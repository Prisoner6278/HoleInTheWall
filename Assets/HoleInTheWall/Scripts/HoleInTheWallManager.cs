using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HoleInTheWallManager : MonoBehaviour
{
    //Fail Counter
    private int fail = 0;

    public GameObject GameOverUI;

    [SerializeField] private PoseJointPoints[] poses;
    private List<int> completedPoses = new List<int>();
    private int index = 0;
    private PoseJointPoints currentPose;

    private bool poseActive;
    [SerializeField] private float timeToMatchThePose;
    private float matchTimer;
    //On success, the timeToMatchPose will be decreased by this value
    [SerializeField] private float timeToMatchPoseChange;
    //The value that the timeToMatchThePose will not go below; value clamp
    [SerializeField] private float timeToMatchThePoseClamp;

    [SerializeField] private float timeBetweenPoses;
    [SerializeField] private float timeBetweenPosesChange;
    [SerializeField] private float timeBetweenPosesClamp;
    private float betweenPosesTimer;

    [SerializeField] private TextMeshProUGUI resultTextBox;
    [SerializeField] private TextMeshProUGUI timerTextBox;

    private bool isTutorial = true;

    private bool failed = false;
    [SerializeField] private SoundCue failSoundCue;

    private void Start()
    {
  
    }

    private void PickPose()
    {
        if(completedPoses.Count == poses.Length)
        {
            completedPoses.Clear();
            index = Random.Range(0, poses.Length);
        }
        while(completedPoses.Contains(index))
        {
            index = Random.Range(0, poses.Length);
        }
        
        currentPose = poses[index];
        currentPose.gameObject.SetActive(true);

        poseActive = true;
        betweenPosesTimer = 0f;
        HideResult();
    }

    private void Update()
    {
        if (failed)
            return;

        if (fail == 2)
        {
            //Work around to not use the AudioManager, idk how it's
            //setup and it gets glitched when the player loses, so I'm
            //gonna just use my own thing
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.MusicStopLoop();
            AudioManager.Instance.gameObject.SetActive(false);

            failSoundCue.PlayOneShot();

            GameOverUI.SetActive(true);

            failed = true;
            return;
        }

        if (!poseActive)
        {
            betweenPosesTimer += Time.deltaTime;
            timerTextBox.text = Mathf.RoundToInt(timeBetweenPoses - betweenPosesTimer).ToString();
            if (betweenPosesTimer >= timeBetweenPoses)
            {
                PickPose();
            }
            return;
        }

        if(isTutorial)
        {
            
            DisplayTutorialText();
            if (MatchCheck())
            {
                AudioManager.Instance.MusicStopLoop();
                isTutorial = false;
                EndPose();
            }
            return;
        }

        if (MatchCheck())
        {
            EndPose();
        }

        matchTimer += Time.deltaTime;
        timerTextBox.text = Mathf.RoundToInt(timeToMatchThePose - matchTimer).ToString();
        if (matchTimer >= timeToMatchThePose)
            EndPose();
    }

    private void EndPose()
    {
        poseActive = false;
        matchTimer = 0;

        bool result = MatchCheck();

        if(result)
        {
            PoseSuccess();
        }
        DisplayResult(result);
        currentPose.gameObject.SetActive(false);
    }

    private void PoseSuccess()
    {
        completedPoses.Add(index);

        timeToMatchThePose -= timeToMatchPoseChange;
        Mathf.Clamp(timeToMatchThePose, timeToMatchThePoseClamp, Mathf.Infinity);

        timeBetweenPoses -= timeBetweenPosesChange;
        Mathf.Clamp(timeBetweenPoses, timeBetweenPosesClamp, Mathf.Infinity);
    }

    private bool MatchCheck()
    {
        return currentPose.Compare();
    }

    private void DisplayTutorialText()
    {
        resultTextBox.gameObject.SetActive(true);
        resultTextBox.text = "Match the pose to start!";
    }

    private void DisplayResult(bool result)
    {
        resultTextBox.gameObject.SetActive(true);
        if(result)
        {
            AudioManager.Instance.PlaySFX("True");
            resultTextBox.text = "Pose is a match!";
        }
        else
        {
            fail += 1;
            resultTextBox.text = "Failed.";

            //Work around to not use the AudioManager, idk how it's
            //setup and it gets glitched when the player loses, so I'm
            //gonna just use my own thing
            if (fail < 2)
            {
                AudioManager.Instance.PlaySFX("False");
            }
        }
    }

    private void HideResult()
    {
        resultTextBox.gameObject.SetActive(false);
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        fail = 0;
        SceneManager.LoadScene(2);
        GameOverUI.SetActive(false);
    }
}
