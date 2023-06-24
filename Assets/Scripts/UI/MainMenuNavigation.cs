using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuNavigation : MonoBehaviour
{
    public void LoadBucketGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadScratchOff()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadHoleInTheWall()
    {
        SceneManager.LoadScene(2);
    }
}
