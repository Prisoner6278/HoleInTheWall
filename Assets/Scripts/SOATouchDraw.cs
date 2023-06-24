using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOATouchDraw : MonoBehaviour
{
    Coroutine drawing;
    public GameObject linePrefab;
    public Camera mainCam;
    public static List<LineRenderer> drawnLineRenderers = new List<LineRenderer>();
    public ScratchOffAttempt scratchScript;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StartLine();    
        }
        if(Input.GetMouseButtonUp(0))
        {
            FinishLine();
        }
    }

    public void StartLine()
    {
        if(drawing != null)
        {
            StopCoroutine(drawing);
        }
        drawing = StartCoroutine(DrawLine());
    }


    public void FinishLine()
    {
        if(drawing!= null)
        {
            StopCoroutine(drawing);
        }
    }

    IEnumerator DrawLine()
    {
        GameObject newGameObject = Instantiate(linePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        LineRenderer line = newGameObject.GetComponent<LineRenderer>();
        drawnLineRenderers.Add(line);
        line.positionCount = 0;
        while(true)
        {
            Vector3 position = mainCam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, position);
            scratchScript.AssignScreenAsMask();
            yield return null;
        }
    }
}
