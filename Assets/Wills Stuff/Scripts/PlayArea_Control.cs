using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayArea_Control : MonoBehaviour
{
    public GameObject CamProjection;
    private MeshRenderer CamProjRenderer;

    private MeshRenderer PlayAreaRenderer;

    // Start is called before the first frame update
    void Start()
    {
        CamProjRenderer = CamProjection.GetComponent<MeshRenderer>();
        PlayAreaRenderer = gameObject.GetComponent<MeshRenderer>();

        //PlayAreaRenderer.material = CamProjRenderer.material;
        //PlayAreaRenderer.material.color = new Color(CamProjRenderer.material.color.r, CamProjRenderer.material.color.g, CamProjRenderer.material.color.b, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        PlayAreaRenderer.material.mainTexture = CamProjRenderer.material.mainTexture;
    }
}
