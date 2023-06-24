using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWebcamToWorldSpace : MonoBehaviour
{
    /**The webcam points range from 0 to 960 on X
     * and 0 to 540 on Y. World space ranges from
     * -480 to 480 on X and -270 to 270 on Y. Both
     * are 16:9, and 480 * 2 = 960 and 270 * 2 = 540.
    */

    public void SetPosition(double x, double y)
    {
        float tempX = (float)x;
        float worldSpaceX = 0;

        worldSpaceX = (tempX / 960) * (480 - (-480)) - 480;


        float tempY = (float)(y);
        float worldSpaceY = 0;

        worldSpaceY = (1 - tempY / 540) * (270 - (-270)) - 270;


        Vector3 position = new Vector3(worldSpaceX, worldSpaceY, transform.position.z);
        transform.position = position;
    }
}
