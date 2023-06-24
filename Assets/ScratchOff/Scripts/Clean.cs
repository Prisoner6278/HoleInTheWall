using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clean : MonoBehaviour
{
    //  This script deals with cleaning the dirt texture with mask 
    // [SerializeField] is used to show private var in inspector, but still cannot be accsesed by other classes.
    //      REF: https://www.youtube.com/watch?v=YfolVBo_2-I 

    //      VARIABLES

    [SerializeField] private Camera _camera;

    [SerializeField] private GameObject soap;
    [SerializeField] private GameObject hitCircle;

    //  Ref to the original dirtMask and brush
    [SerializeField] private Texture2D dirtMaskLayer;
    [SerializeField] private Texture2D brush;

    private float dirtAmountTotal;
    private float dirtFull;

    //  Change brush size
    //[SerializeField] private int brushScale;
    //private int brushWidthScaled;
    //private int brushHeightScaled;

    [SerializeField] private Material material;

    //  The created dirtMask we will use
    private Texture2D templatedirtMask;

    // Start is called before the first frame update
    void Start()
    {
        CreateTexture();

        //  Change brush size
        //brushWidthScaled = brush.width * brushScale;
        //brushHeightScaled = brush.height * brushScale;
    }

    // Update is called once per frame
    void Update()
    {

        //  Mouse collider
        hitCircle.transform.position = _camera.ScreenToWorldPoint(Input.mousePosition);
        
        //  Only get the 'Balls' or 6 layermask, but need to bishift to it <<
        int layerMask = 1 << 6;
        // ~ inverts bitmask
        layerMask = ~layerMask;

        //  Hold mouse button to clean
        //  DO NOT NEED, PLAYER WILL ALWAYS BE CLEANING 
        //if(Input.GetMouseButton(0))
        //{
        //  Raycast from camera, to mouse position. out means this is the var the collision result is stored in
        //      out hit simply means hit is the var. RaycastHit is a structure (a collection of one or more var) like type
        //  CHANGE THE INPUT.MOUSEPOSITION TO THE PLAYER HAND POS  Input.mousePosition soap.transform.position
        //              Use WorldToScreenPoint to convert the Vector3 to screenPos, Need distance (Mathf Infinity) and then layermask
        if (Physics.Raycast(_camera.ScreenPointToRay(_camera.WorldToScreenPoint(soap.transform.position))
            , out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                //Debug.Log("Hit"); // Raycast does not collide with 2D colliders, use a 3D object or plane (3D Collider).  
                //  Save the hit info of textureCoord to textureCoord local var
                Vector2 textureCoord = hit.textureCoord;

                //  Convert from UV coords to Pixel coords
                int pixelX = (int)(textureCoord.x * templatedirtMask.width);
                int pixelY = (int)(textureCoord.y * templatedirtMask.height);

                //Debug.Log(pixelX + ", " + pixelY);
                //Vector2Int paintPixelPosition = new Vector2Int(pixelX, pixelY);

                //  Make the center of the brush 
                int pixelOffsetX = pixelX - (brush.width / 2);
                int pixelOffsetY = pixelY - (brush.height / 2);

                //  Interact with the selected pixels in the brush area
                //
                for (int x = 0; x < brush.width; x++)
                {
                    for (int y = 0; y < brush.height; y++)
                    {
                        //  Get all pixels in the brush
                        Color pixelDirt = brush.GetPixel(x, y);
                        //  Get 
                        Color pixelDirtMask = templatedirtMask.GetPixel(pixelOffsetX + x, pixelOffsetY + y);

                        //  Set the pixels in the brush area and mouse, brush gives falloff for intensity 
                        //      I think this is 'removing' the green mask layer, which then shows the clean layer under
                        //      Using a multiply so the black brush circle is the new color, which reveals the other layer
                        templatedirtMask.SetPixel(pixelOffsetX + x, pixelOffsetY + y, new Color(0, pixelDirtMask.g * pixelDirt.g, 0));
                        
                    }
                }

                templatedirtMask.Apply();

            }

        //}

        //  Check the mask percentage
        //CheckMaskPercent();



    }

    private void CreateTexture()
    {
        //  Setting template to have the same size at the dirtMask
        templatedirtMask = new Texture2D(dirtMaskLayer.width, dirtMaskLayer.height);
        //  Setting the template to have the same colors as dirtMask
        //      Need to go into the texture asset and check Read/Write under advanced
        templatedirtMask.SetPixels(dirtMaskLayer.GetPixels());
        //  Need Apply to save changes above
        templatedirtMask.Apply();
        
        //      NEED TO CHANGE THIS IF I CHANGE THE SHADER I AM USING
        //  Need to use the shader ID name, can be found by clicking on the var in the shadergraph
        material.SetTexture("_Mask", templatedirtMask);
        //  Need to set _DirtMask, as this is the area we want to paint (Green Square Mask)

        //  Get the full amount of dirt
        //dirtFull = 0;
        //for (int x = 0; x < dirtMaskLayer.width; x++)
        //{
        //    for (int y = 0; y < dirtMaskLayer.height; y++)
        //    {
        //        dirtFull += dirtMaskLayer.GetPixel(x, y).g;
        //        Debug.Log(dirtFull);
        //    }
        //}
    }

    private void CheckMaskPercent()
    {
        //  Reset the counter
        //dirtAmountTotal = 0;
        //  Cycle through all pixles of the templatedirtMask, adding 1 for each .g channel pixel (so where the mask is)
        //for (int x = 0; x < templatedirtMask.width; x++)
        //{
        //    for (int y = 0; y < templatedirtMask.height; y++)
        //    {
        //        dirtAmountTotal += templatedirtMask.GetPixel(x, y).g;
        //    }
        //}
    }
}


