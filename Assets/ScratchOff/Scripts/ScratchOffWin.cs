using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratchOffWin : MonoBehaviour
{
    [SerializeField] private Transform topLeft;
    [SerializeField] private Transform bottomRight;

    [SerializeField] private GameObject scratchOffRegionPrefab;

    //Must be equal to or greater than 2
    //Will be set to an even number for simplicity
    [SerializeField] private int screenDivisions = 2;

    private int regionsCreated;
    private int regionsActive = 0;
    [SerializeField] private float percentCleanedRequired;

    [SerializeField] ScratchOffManager manager;
    private bool hasWon = false;

    private void Start()
    {
        if(screenDivisions <= 1)
        {
            screenDivisions = 2;
            Debug.LogWarning("Screen Divisions must be greater than or equal to 2, value set to 2");
        }
        else if(screenDivisions % 2 != 0)
        {
            screenDivisions += 1;
            Debug.Log("Screen Divisions value has been set to an even number for simplicity");
        }

        if(percentCleanedRequired >= 1 || percentCleanedRequired <= 0)
        {
            percentCleanedRequired = .9f;
            Debug.LogWarning("Percent Cleaned Required should fall into a range of 0 to 1");
        }

        DivideScreen();
    }

    private void DivideScreen()
    {
        regionsCreated = screenDivisions * screenDivisions;

        for (int i = 0; i < screenDivisions; i++)
        {
            float xPercent = ((float)i * 2 + 1) / (screenDivisions * 2);

            for (int j = 0; j < screenDivisions; j++)
            {
                float yPercent = ((float)j * 2 + 1) / (screenDivisions * 2);

                float xPos = Mathf.Lerp(topLeft.position.x, bottomRight.position.x, xPercent);
                float yPos = Mathf.Lerp(topLeft.position.y, bottomRight.position.y, yPercent);

                Vector3 pos = new Vector3(xPos, yPos, 0);
                GameObject region = Instantiate(scratchOffRegionPrefab, transform);
                region.transform.position = pos;

                ScratchOffRegion scratchOffRegion = region.GetComponent<ScratchOffRegion>();
                scratchOffRegion.scratchOff = this;
            }
        }
    }

    public void ActiveRegion()
    {
        regionsActive += 1;
        CheckIfComplete();
    }

    private void CheckIfComplete()
    {
        if (hasWon)
            return;
        
        if(!((float)regionsActive / (float)regionsCreated >= percentCleanedRequired))
        {
            return;
        }

        hasWon = true;
        manager.Win();
    }
}
