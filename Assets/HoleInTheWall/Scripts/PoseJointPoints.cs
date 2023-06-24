using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using OpenCVForUnityExample;
using OpenCVForUnity.CoreModule;

public class PoseJointPoints : MonoBehaviour
{
    [field:SerializeField] private WebcamPoints webcam;

    [Header("Joint GFX")]
    [SerializeField] private Color correctColor;
    [SerializeField] private Color wrongColor;

    [field:Header("Joints")]
    [field:SerializeField] public JointGrade Torso { get; private set; }
    [field:SerializeField] public JointGrade LeftArm { get; private set; }
    [field:SerializeField] public JointGrade RightArm { get; private set; }
    [field:SerializeField] public JointGrade LeftLeg { get; private set; }
    [field:SerializeField] public JointGrade RightLeg { get; private set; }

    private JointGrade[] jointGrades;

    private void Awake()
    {
        jointGrades = new JointGrade[] { LeftArm, RightArm, Torso, LeftLeg, RightLeg };
    }

    public bool Compare()
    {
        bool allJointsMatch = true;
        foreach (JointGrade jointGrade in jointGrades)
        {
            foreach (PointComparisons comparison in jointGrade.PointComparisons)
            {
                string aString = comparison.a.name;
                string bString = comparison.b.name;
                Point aPoint = webcam.points[webcam.BODY_PARTS.GetValueOrDefault(aString)];
                Point bPoint = webcam.points[webcam.BODY_PARTS.GetValueOrDefault(bString)];
                Vector3 a = webcam.WebcamToWorld(aPoint);
                Vector3 b = webcam.WebcamToWorld(bPoint);
                //if(aString.CompareTo("LKnee") == 1)
                //{
                //    Debug.Log($"{a}, {b}");
                //}
                //Debug.Log($"{aString}: {comparison.a.position}, {bString}: {comparison.b.position}");
                //Debug.Log($"{aString}: {a}, {bString}: {b}");

                ComparisonValue poseX;
                ComparisonValue poseY;

                ComparisonValue webcamX;
                ComparisonValue webcamY;

                //Compare Point & Webcam X
                if (comparison.CompareX)
                {
                    //Point compare
                    if (comparison.a.position.x > comparison.b.position.x)
                    {
                        poseX = ComparisonValue.GreaterThan;
                    }
                    else if (comparison.a.position.x < comparison.b.position.x)
                    {
                        poseX = ComparisonValue.LessThan;
                    }
                    else
                    {
                        poseX = ComparisonValue.EqualTo;
                    }

                    //Webcam compare
                    if (a.x > b.x)
                    {
                        webcamX = ComparisonValue.GreaterThan;
                    }
                    else if (a.x < b.x)
                    {
                        webcamX = ComparisonValue.LessThan;
                    }
                    else
                    {
                        webcamX = ComparisonValue.EqualTo;
                    }

                    //Point & Webcam compare
                    if (poseX != webcamX)
                    {
                        jointGrade.jointMatchesPose = false;
                        break;
                    }
                }

                //Compare Point & Webcam Y
                if(comparison.CompareY)
                {
                    //Point compare
                    if (comparison.a.position.y > comparison.b.position.y)
                    {
                        poseY = ComparisonValue.GreaterThan;
                    }
                    else if (comparison.a.position.y < comparison.b.position.y)
                    {
                        poseY = ComparisonValue.LessThan;
                    }
                    else
                    {
                        poseY = ComparisonValue.EqualTo;
                    }

                    //Webcam compare
                    if (a.y > b.y)
                    {
                        webcamY = ComparisonValue.GreaterThan;
                    }
                    else if(a.y < b.y)
                    {
                        webcamY = ComparisonValue.LessThan;
                    }
                    else
                    {
                        webcamY = ComparisonValue.EqualTo;
                    }

                    //Point & Webcam compare
                    if (poseY != webcamY)
                    {
                        jointGrade.jointMatchesPose = false;
                        break;
                    }
                }

                jointGrade.jointMatchesPose = true;
            }

            if(jointGrade.jointMatchesPose)
            {
                jointGrade.JointGFX.color = correctColor;
            }
            else
            {
                jointGrade.JointGFX.color = wrongColor;
                allJointsMatch = false;
            }
        }

        return allJointsMatch;
    }
}

[Serializable]
public class PointComparisons
{
    [field:SerializeField] public bool CompareX { get; private set; }
    [field:SerializeField] public bool CompareY { get; private set; }
    public Transform a;
    public Transform b;
}

public enum ComparisonValue
{
    GreaterThan = 0,
    LessThan = 1,
    EqualTo = 2
}


[Serializable]
public class JointGrade
{
    [field:SerializeField] public SpriteRenderer JointGFX { get; private set; }
    [field:SerializeField] public PointComparisons[] PointComparisons { get; private set; }
    [HideInInspector] public bool jointMatchesPose = true;
}
