using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizedClean : MonoBehaviour
{
    //  Test of Optimization, dealing with alpha to allow for the bottom image to be animated. 

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

    //  For the top layer
    [SerializeField] private Texture2D topLayer;

    private float dirtAmountTotal;
    private float dirtAmount;

    [SerializeField] private float waterCapacity;
    private float currentWaterCapacity;
    [SerializeField] private float waterDepletionRate;

    // Percentage of cleared "dirt" needed for win state (Max 100)
    [SerializeField] private int targetCleanness;

    //  Change brush size
    //[SerializeField] private int brushScale;
    //private int brushWidthScaled;
    //private int brushHeightScaled;

    [SerializeField] private Material material;

    //used for managing win state
    [SerializeField] ScratchOffManager manager;

    //  The created dirtMask we will use
    private Texture2D templatedirtMask;

    int brushBaseHeight;
    int brushBaseWidth;

    //  Timer for raycast
    public double interval = 0.4;
    private double timer = 0.0;
    private bool hasHit = false;

    //Webcam Setup
    [SerializeField] private Transform rHand;
    [SerializeField] private bool followWebcam;

    // Start is called before the first frame update
    void Start()
    {
        //  declare timer
        timer = interval + 1.0;

        brushBaseHeight = brush.height;
        brushBaseWidth = brush.width;

        //CreateTexture();

        //dirtAmountTotal = 0f;
        //for (int x = 0; x < templatedirtMask.width; x++)
        //{
        //    for (int y = 0; y < templatedirtMask.height; y++)
        //    {
        //        dirtAmountTotal += templatedirtMask.GetPixel(x, y).a;
        //    }
        //}
        //dirtAmount = dirtAmountTotal;
        //
        //currentWaterCapacity = waterCapacity;

        //  Change brush size
        //brushWidthScaled = brush.width * brushScale;
        //brushHeightScaled = brush.height * brushScale;
    }

    public void RunTexture()
    {
        dirtAmountTotal = 0f;
        for (int x = 0; x < templatedirtMask.width; x++)
        {
            for (int y = 0; y < templatedirtMask.height; y++)
            {
                dirtAmountTotal += templatedirtMask.GetPixel(x, y).a;
            }
        }
        dirtAmount = dirtAmountTotal;

        currentWaterCapacity = waterCapacity;
    }

    //  Used to set a new texture from the ScratchOffManager script randomization
    public void SetTexture(Texture2D texture)
    {
        topLayer = texture;
    }

    public void SetMat(Material mat)
    {
        material = mat;
        CreateTexture();
    }

    // Update is called once per frame
    void Update()
    {

        // adding a timer on raycast
        double usedInterval = interval;
        if (Time.deltaTime > usedInterval)
        {
            usedInterval = Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(Mathf.RoundToInt(GetCleanAmount() * 100f) + "%");
        }

        //  Mouse collider
        if(followWebcam)
        {
            Vector3 position = new Vector3(rHand.transform.position.x, rHand.transform.position.y, -10);
            float xClamp = Mathf.Clamp(position.x, -16, 16); 
            float yClamp = Mathf.Clamp(position.y, -7.8f, 9.8f);
            position = new Vector3(xClamp, yClamp, -10);

            hitCircle.transform.position = position;
            Debug.Log($"{position}");
        }
        else
        {
            hitCircle.transform.position = _camera.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(hitCircle.transform.position);
        }

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
        
        if (!hasHit && timer >= usedInterval)
        {
            timer = 0;


            if (Physics.Raycast(_camera.ScreenPointToRay(_camera.WorldToScreenPoint(hitCircle.transform.position))
            , out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                hasHit = true;
                //  Commented out the water refill since it was not working for me
                if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit refillHit))
                {
                    if (refillHit.collider.CompareTag("ScratchOffRefill"))
                    {
                        Debug.Log("water refilled");
                        currentWaterCapacity = waterCapacity;
                        brush = ScaleTexture(brush, brushBaseWidth, brushBaseHeight);
                    }
                }

                if (Mathf.InverseLerp(waterCapacity, 0, currentWaterCapacity) > 0.9f)
                {
                    float scaledSize = superScale(Mathf.Lerp(waterCapacity, 0, 0.9f), 0, brushBaseHeight, 0, currentWaterCapacity);
                    brush = ScaleTexture(brush, (int)scaledSize, (int)scaledSize);
                }
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

                pixelOffsetX = Mathf.Clamp(pixelOffsetX, 0, topLayer.width);
                pixelOffsetY = Mathf.Clamp(pixelOffsetY, 0, topLayer.height);

                //  Interact with the selected pixels in the brush area
                //  If water is not empty

                if (currentWaterCapacity > 0)
                {
                    //  Get all brush pixels
                    //Color[] brushPixels = brush.GetPixels(0, 0, brush.width, brush.height);
                    //  Get all the image pixels within the brush size
                    //Color[] imagePixels = templatedirtMask.GetPixels(pixelOffsetX, pixelOffsetY, brush.width, brush.height);

                    //templatedirtMask.SetPixels(pixelOffsetX, pixelOffsetY, brush.width, brush.height, brushPixels);
                    //templatedirtMask.SetPixels(pixelX, pixelY, brush.width, brush.height, brushPixels);



                    for (int x = 0; x < brush.width; x++)
                    {
                        for (int y = 0; y < brush.height; y++)
                        {
                            //  Get pixel color in the brush
                            Color pixelDirt = brush.GetPixel(x, y);
                            //  Get 
                            Color pixelDirtMask = templatedirtMask.GetPixel(pixelOffsetX + x, pixelOffsetY + y);

                            //  Reduce water
                            currentWaterCapacity -= waterDepletionRate;

                            //calculate removed amount of dirt and subtract removed amount from total amount
                            float removedAmount = pixelDirtMask.a - (pixelDirtMask.a * pixelDirt.a);
                            dirtAmount -= removedAmount;

                            //  Only changing the a (Alpha), need to multiply pixel dirt (brush) by pixelDirtMask to get falloff of alpha.
                            templatedirtMask.SetPixel(pixelOffsetX + x, pixelOffsetY + y, new Color(pixelDirtMask.r, pixelDirtMask.g, pixelDirtMask.b, pixelDirtMask.a * pixelDirt.a));
                        }
                    }

                }

                templatedirtMask.Apply();
                
            }

            hasHit = false;

        }

        timer += Time.deltaTime;

        //}

        //  Check the mask percentage
        //CheckMaskPercent();
    }

    private void FixedUpdate()
    {
        CheckforWin();
    }

    private void CreateTexture()
    {
        //  Setting template to have the same size at the dirtMask
        templatedirtMask = new Texture2D(topLayer.width, topLayer.height);
        //  Setting the template to have the same colors as dirtMask
        //      Need to go into the texture asset and check Read/Write under advanced
        templatedirtMask.SetPixels(topLayer.GetPixels());
        //  Need Apply to save changes above
        templatedirtMask.Apply();

        //      NEED TO CHANGE THIS IF I CHANGE THE SHADER I AM USING
        //  Need to use the shader ID name, can be found by clicking on the var in the shadergraph
        material.SetTexture("_Mask", templatedirtMask);
        //  Need to set _DirtMask, as this is the area we want to paint (Green Square Mask)


        RunTexture();

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

    private float GetCleanAmount()
    {
        return (1 - this.dirtAmount / dirtAmountTotal);
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

    private void CheckforWin()
    {
        if (Mathf.RoundToInt(GetCleanAmount() * 100f) >= targetCleanness)
        {
            manager.Win();
        }
    }
    
    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        return result;
    }

    public float superScale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }
    
}
