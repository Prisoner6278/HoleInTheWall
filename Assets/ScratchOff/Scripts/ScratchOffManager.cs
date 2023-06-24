using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScratchOffManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI;

    //  Make sure each value is the same (ie fish top and bottom are both 0 in array)
    [SerializeField] public Texture2D[] topTexture;
    [SerializeField] private Material[] topMats;
    [SerializeField] private Material[] bottomMats;

    [SerializeField] private GameObject topPlane;
    [SerializeField] private GameObject bottomPlane;

    [SerializeField] AlphaClean alphaClean;
    [SerializeField] OptimizedClean optimizedClean;

    private bool won;

    private void Start()
    {
        Randomize();

        //AudioManager.Instance.PlayMusic("MainTheme");
    }

    private void Update()
    {
        //Randomize();
    }

    //  Choose a pair of top and bottom layers to apply to the planes
    public void Randomize()
    {
        Debug.Log("Randomizing");
        int selection = Random.Range(0, topTexture.Length);
        Debug.Log(selection);

        topPlane.gameObject.GetComponent<MeshRenderer>().material = topMats[selection];
        bottomPlane.gameObject.GetComponent<MeshRenderer>().material = bottomMats[selection];


        optimizedClean.SetTexture(topTexture[selection]);
        optimizedClean.SetMat(topMats[selection]);
        //alphaClean.SetTexture(topTexture[selection]);
        //alphaClean.SetMat(topMats[selection]);

        // choose a Top and Bottom layer, random.range(0, layerPair.length -1)

        //  Need to pass the top mat to Top Plane, and topLayer 2D var to AlphaClean script on Top Plane
        //  Need to pass only mat to Bottom Plane
    }

    public void Win()
    {
        if (won)
            return;

        won = true;
        AudioManager.Instance.PlaySFX("Win");
        gameOverUI.SetActive(true);
    }

    public void Replay()
    {
        won = false;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }
}
