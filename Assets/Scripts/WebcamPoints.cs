#if !(PLATFORM_LUMIN && !UNITY_EDITOR)

#if !UNITY_WSA_10_0

using OpenCVForUnity.CoreModule;
using OpenCVForUnity.DnnModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UnityUtils.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OpenCVForUnityExample
{
    /// <summary>
    /// LightweightPoseEstimation WebCam Example
    /// An example of DNN inference with a lightweight person pose estimation model using the KeypointsModel class.
    /// </summary>
    [RequireComponent(typeof(WebCamTextureToMatHelper))]
    public class WebcamPoints : MonoBehaviour
    {
        [SerializeField] private float universalScaleDivision = 1;
        [SerializeField] private bool hideHitboxes;
        [SerializeField] private bool hideWebcam;
        [SerializeField] private float cameraSize = 240;

        [SerializeField] private bool drawDebug = true;
        
        [Header("Lerp")]
        [SerializeField] private bool useLerp;
        [SerializeField] private float maxDelta;
        [SerializeField] private float rotationSpeed = 1; 

        [Header("Limbs")]
        [SerializeField] private float objectScale; 

        [SerializeField] private bool foreArm;
        private GameObject foreArmRight;
        private GameObject foreArmLeft;


        [SerializeField] private bool arm;
        private GameObject armRight;
        private GameObject armLeft;

        public List<Point> points = new List<Point>();

        public Vector3 LeftWristScreen { get; private set; }
        public Vector3 RightWristScreen { get; private set; }

        [SerializeField]
        public Dictionary<string, Point> BodyPartData = new Dictionary<string, Point>();

        public Mat imgRef;

        /// <summary>
        /// The texture.
        /// </summary>
        Texture2D texture;

        /// <summary>
        /// The webcam texture to mat helper.
        /// </summary>
        WebCamTextureToMatHelper webCamTextureToMatHelper;

        /// <summary>
        /// The FPS monitor.
        /// </summary>
        FpsMonitor fpsMonitor;


        Net net;

        KeypointsModel keypointsModel;

        const float inWidth = 256;
        const float inHeight = 256;

        const float inScale = 1.0f / 255f;

        double[] inMean = new double[] { 128.0, 128.0, 128.0 };

        double threshold = 0.1;

        public Dictionary<string, int> BODY_PARTS = new Dictionary<string, int>() {
                        { "Nose", 0 }, { "Neck", 1 }, { "LShoulder", 2 }, { "LElbow", 3 }, {
                            "LWrist",
                            4
                        },
                        { "RShoulder",5 }, { "RElbow", 6 }, { "RWrist", 7 }, { "LHip", 8 }, {
                            "LKnee",
                            9
                        },
                        { "LAnkle", 10 }, { "RHip", 11 }, { "RKnee", 12 }, { "RAnkle", 13 }, {
                            "REye",
                            14
                        },
                        { "LEye", 15 }, { "REar", 16 }, { "LEar", 17 }, {
                            "Background",
                            18
                        }
                    };

        string[,] POSE_PAIRS = new string[,] {
                        { "Neck", "RShoulder" }, { "Neck", "LShoulder" }, {
                            "RShoulder",
                            "RElbow"
                        },
                        { "RElbow", "RWrist" }, { "LShoulder", "LElbow" }, {
                            "LElbow",
                            "LWrist"
                        },
                        { "Neck", "RHip" }, { "RHip", "RKnee" }, { "RKnee", "RAnkle" }, {
                            "Neck",
                            "LHip"
                        },
                        { "LHip", "LKnee" }, { "LKnee", "LAnkle" }, { "Neck", "Nose" }, {
                            "Nose",
                            "REye"
                        }//,
                       // { "REye", "REar" }, { "Nose", "LEye" }, { "LEye", "LEar" }
            };


        /// <summary>
        /// MODEL_FILENAME
        /// </summary>
        string MODEL_FILENAME = "OpenCVForUnity/dnn/lightweight_pose_estimation_201912.onnx";

        /// <summary>
        /// The model filepath.
        /// </summary>
        string model_filepath;

#if UNITY_WEBGL
        IEnumerator getFilePath_Coroutine;
#endif

        // Use this for initialization
        void Start()
        {
            fpsMonitor = GetComponent<FpsMonitor>();

            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();


            Point dummy = new Point();

            BodyPartData = new Dictionary<string, Point>()
            {
                {"Nose", dummy}, 
                {"Neck", dummy},
                {"RShoulder", dummy}, {"RElbow", dummy}, {"RWrist", dummy},
                {"LShoulder", dummy}, {"LElbow", dummy}, {"LWrist", dummy}, {"RHip", dummy}, {"RKnee", dummy},
                {"RAnkle", dummy}, {"LHip", dummy}, {"LKnee", dummy}, {"LAnkle", dummy},
                {"REye", dummy}, {"LEye", dummy}, {"REar", dummy}, {"LEar", dummy},
                {"Background", dummy}
            };



#if UNITY_WEBGL
            getFilePath_Coroutine = GetFilePath();
            StartCoroutine(getFilePath_Coroutine);
#else
            model_filepath = Utils.getFilePath(MODEL_FILENAME);
            Run();
#endif
        }

#if UNITY_WEBGL
        private IEnumerator GetFilePath()
        {

            var getFilePathAsync_0_Coroutine = Utils.getFilePathAsync(MODEL_FILENAME, (result) =>
            {
                model_filepath = result;
            });
            yield return getFilePathAsync_0_Coroutine;

            getFilePath_Coroutine = null;

            Run();
        }
#endif

        // Use this for initialization
        void Run()
        {

            //if true, The error log of the Native side OpenCV will be displayed on the Unity Editor Console.
            Utils.setDebugMode(true);

            net = null;

            if (string.IsNullOrEmpty(model_filepath))
            {
                Debug.LogError(MODEL_FILENAME + " is not loaded. Please read “StreamingAssets/OpenCVForUnity/dnn/setup_dnn_module.pdf” to make the necessary setup.");
            }
            else
            {
                net = Dnn.readNet(model_filepath);

                keypointsModel = new KeypointsModel(net);
                keypointsModel.setInputScale(inScale);
                keypointsModel.setInputSize(new Size(inWidth, inHeight));
                keypointsModel.setInputMean(new Scalar(inMean));
                keypointsModel.setInputSwapRB(false);
                keypointsModel.setInputCrop(false);
            }


            Utils.setDebugMode(false);

            webCamTextureToMatHelper.Initialize();

            Camera.main.orthographicSize = cameraSize;

            if (hideWebcam)
                GetComponent<MeshRenderer>().enabled = false;

            if (arm)
                InitializeArm();

            if (foreArm)
                InitializeForeArm();
        }

        /// <summary>
        /// Raises the webcam texture to mat helper initialized event.
        /// </summary>
        public void OnWebCamTextureToMatHelperInitialized()
        {
            Debug.Log("OnWebCamTextureToMatHelperInitialized");

            Mat webCamTextureMat = webCamTextureToMatHelper.GetMat();

            texture = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGB24, false);
            Utils.matToTexture2D(webCamTextureMat, texture);

            gameObject.GetComponent<Renderer>().material.mainTexture = texture;

            gameObject.transform.localScale = new Vector3(webCamTextureMat.cols(), webCamTextureMat.rows(), 1);
            Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

            if (fpsMonitor != null)
            {
                //fpsMonitor.Add("deviceName", webCamTextureToMatHelper.GetDeviceName().ToString());
                fpsMonitor.Add("width", webCamTextureToMatHelper.GetWidth().ToString());
                fpsMonitor.Add("height", webCamTextureToMatHelper.GetHeight().ToString());
                //fpsMonitor.Add("videoRotationAngle", webCamTextureToMatHelper.GetWebCamTexture().videoRotationAngle.ToString());
                //fpsMonitor.Add("videoVerticallyMirrored", webCamTextureToMatHelper.GetWebCamTexture().videoVerticallyMirrored.ToString());
                //fpsMonitor.Add("camera fps", webCamTextureToMatHelper.GetFPS().ToString());
                //fpsMonitor.Add("isFrontFacing", webCamTextureToMatHelper.IsFrontFacing().ToString());
                //fpsMonitor.Add("rotate90Degree", webCamTextureToMatHelper.rotate90Degree.ToString());
                //fpsMonitor.Add("flipVertical", webCamTextureToMatHelper.flipVertical.ToString());
                //fpsMonitor.Add("flipHorizontal", webCamTextureToMatHelper.flipHorizontal.ToString());
                fpsMonitor.Add("orientation", Screen.orientation.ToString());
            }


            float width = webCamTextureMat.width();
            float height = webCamTextureMat.height();

            
            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale)
            {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            }
            else
            {
                Camera.main.orthographicSize = height / 2;
            }
            
            
        }

        /// <summary>
        /// Raises the webcam texture to mat helper disposed event.
        /// </summary>
        public void OnWebCamTextureToMatHelperDisposed()
        {
            Debug.Log("OnWebCamTextureToMatHelperDisposed");

            if (texture != null)
            {
                Texture2D.Destroy(texture);
                texture = null;
            }
        }

        /// <summary>
        /// Raises the webcam texture to mat helper error occurred event.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        public void OnWebCamTextureToMatHelperErrorOccurred(WebCamTextureToMatHelper.ErrorCode errorCode)
        {
            Debug.Log("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);

            if (fpsMonitor != null)
            {
                fpsMonitor.consoleText = "ErrorCode: " + errorCode;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
            {

                Mat img = webCamTextureToMatHelper.GetMat();
                imgRef = img;

                if (net == null)
                {
                    Imgproc.putText(img, "model file is not loaded.", new Point(5, img.rows() - 30), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255), 2, Imgproc.LINE_AA, false);
                    Imgproc.putText(img, "Please read console message.", new Point(5, img.rows() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255), 2, Imgproc.LINE_AA, false);
                }
                else
                {
                    points = keypointsModel.estimate(img, (float)threshold).toList();

                    RightWristScreen = Camera.main.WorldToScreenPoint(WebcamToWorld(points[7]) / universalScaleDivision);
                    LeftWristScreen = Camera.main.WorldToScreenPoint(WebcamToWorld(points[4]) / universalScaleDivision);

                    for (int i = 0; i < POSE_PAIRS.GetLength(0); i++)
                    {
                        string partFrom = POSE_PAIRS[i, 0];
                        string partTo = POSE_PAIRS[i, 1];

                        int idFrom = BODY_PARTS[partFrom];
                        int idTo = BODY_PARTS[partTo];
                        
                        BodyPartData[partTo] = points[idTo];
                        

                        if (points[idFrom] != null && points[idTo] != null)
                        {

                            if (drawDebug)
                            {
                                Imgproc.line(img, points[idFrom], points[idTo], new Scalar(0, 255, 0), 3);
                                Imgproc.ellipse(img, points[idFrom], new Size(3, 3), 0, 0, 360, new Scalar(0, 0, 255),
                                    Core.FILLED);
                                Imgproc.ellipse(img, points[idTo], new Size(3, 3), 0, 0, 360, new Scalar(0, 0, 255),
                                    Core.FILLED);
                            }

                            //Debug.Log($"{idFrom}");

                            //Head test
                            //if(idFrom == 0)
                            //{
                            //    Debug.Log($"{points[idFrom].x}, {points[idFrom].y}");

                            //    Imgproc.ellipse(img, points[idFrom], new Size(3, 3), 0, 0, 360, new Scalar(255, 0, 0), Core.FILLED);
                            //    testWorldspace.SetPosition(points[idFrom].x, points[idFrom].y);
                            //}

                            RightWristScreen = Camera.main.WorldToScreenPoint(WebcamToWorld(points[idTo]) / universalScaleDivision);
                            LeftWristScreen = Camera.main.WorldToScreenPoint(WebcamToWorld(points[idTo]) / universalScaleDivision);

                            if (arm)
                            {
                                if (idFrom == 5 && idTo == 6)
                                    LimbUpdate(armRight, points[idFrom], points[idTo]);
                                if (idFrom == 2 && idTo == 3)
                                    LimbUpdate(armLeft, points[idFrom], points[idTo]);
                            }

                            if (foreArm)
                            {
                                if (idFrom == 6 && idTo == 7)
                                {
                                    LimbUpdate(foreArmRight, points[idFrom], points[idTo]);
                                    //RightWristScreen = (WebcamToWorld(points[idTo]) / universalScaleDivision);
                                    //RightWristScreen = Camera.main.WorldToScreenPoint(WebcamToWorld(points[idTo]) / universalScaleDivision);
                                    //RightWristScreen = new Vector3(RightWristScreen.x, Screen.height - RightWristScreen.y, 0);
                                }
                                if (idFrom == 3 && idTo == 4)
                                {
                                    LimbUpdate(foreArmLeft, points[idFrom], points[idTo]);

                                    //LeftWristScreen = Camera.main.WorldToScreenPoint(WebcamToWorld(points[idTo]) / universalScaleDivision);
                                    //LeftWristScreen = new Vector3(LeftWristScreen.x, Screen.height - LeftWristScreen.y, 0);
                                }
                            }
                        }
                    }
                }
                
                

                Imgproc.cvtColor(img, img, Imgproc.COLOR_BGR2RGB);

                //Imgproc.putText (img, "W:" + img.width () + " H:" + img.height () + " SO:" + Screen.orientation, new Point (5, img.rows () - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 1.0, new Scalar (255, 255, 255, 255), 2, Imgproc.LINE_AA, false);

                Utils.matToTexture2D(img, texture);
            }
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy()
        {
            webCamTextureToMatHelper.Dispose();

            if (keypointsModel != null)
                keypointsModel.Dispose();

            if (net != null)
                net.Dispose();

#if UNITY_WEBGL
            if (getFilePath_Coroutine != null)
            {
                StopCoroutine(getFilePath_Coroutine);
                ((IDisposable)getFilePath_Coroutine).Dispose();
            }
#endif
        }

        private void InitializeArm()
        {
            armRight = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            armRight.transform.localScale = new Vector3(objectScale, 1, 1);

            armLeft = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            armLeft.transform.localScale = new Vector3(objectScale, 1, 1);

            if(hideHitboxes)
            {
                armRight.GetComponent<MeshRenderer>().enabled = false;
                armLeft.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        private void InitializeForeArm()
        {
            foreArmRight = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            foreArmRight.transform.localScale = new Vector3(objectScale, 1, 1);

            foreArmLeft = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            foreArmLeft.transform.localScale = new Vector3(objectScale, 1, 1);

            if (hideHitboxes)
            {
                foreArmRight.GetComponent<MeshRenderer>().enabled = false;
                foreArmLeft.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        private void LimbUpdate(GameObject limbToUpdate, Point start, Point end)
        {
            Vector2 location = LocationBetweenPoints(start, end);

            if(useLerp)
            {
                Vector3 target = new Vector3(location.x / universalScaleDivision, location.y / universalScaleDivision, limbToUpdate.transform.position.z);
                float singleStepPosition = maxDelta * Time.deltaTime;
                limbToUpdate.transform.position = Vector3.MoveTowards(limbToUpdate.transform.position, target, singleStepPosition);

                Vector3 currentAngle = limbToUpdate.transform.eulerAngles;
                currentAngle = new Vector3(0, 0, Mathf.LerpAngle(currentAngle.z, (CalculateRotationAngle(start, end) + 90) * -1, Time.deltaTime * rotationSpeed));
                limbToUpdate.transform.eulerAngles = currentAngle;

                limbToUpdate.transform.localScale = new Vector3(objectScale / universalScaleDivision, DistanceBetweenPoints(start, end) / 1.82f / universalScaleDivision, 1);
            }
            else
            {
                //limbToUpdate.transform.position = new Vector3(location.x, location.y, limbToUpdate.transform.position.z);

                //limbToUpdate.transform.eulerAngles = new Vector3(0, 0, (CalculateRotationAngle(start, end) + 90) * -1);

                //limbToUpdate.transform.localScale = new Vector3(objectScale, DistanceBetweenPoints(start, end) / 1.82f, 1);

                limbToUpdate.transform.position = new Vector3(location.x / universalScaleDivision, location.y / universalScaleDivision, limbToUpdate.transform.position.z);

                limbToUpdate.transform.eulerAngles = new Vector3(0, 0, (CalculateRotationAngle(start, end) + 90) * -1);

                limbToUpdate.transform.localScale = new Vector3(objectScale / universalScaleDivision, DistanceBetweenPoints(start, end) / 1.82f / universalScaleDivision, 1);
            }
        }

        public Vector2 WebcamToWorld(Point webcamPoint)
        {
            int webcamX = webCamTextureToMatHelper.GetWidth();
            int webCamXHalf = webcamX / 2;

            float tempX = (float)webcamPoint.x;
            float worldSpaceX = 0;
            worldSpaceX = (tempX / webcamX) * (webCamXHalf - (-webCamXHalf)) - webCamXHalf;


            int webcamY = webCamTextureToMatHelper.GetHeight();
            int webCamYHalf = webcamY / 2;

            float tempY = (float)webcamPoint.y;
            float worldSpaceY = 0;
            worldSpaceY = (1 - tempY / webcamY) * (webCamYHalf - (-webCamYHalf)) - webCamYHalf;

            return new Vector2(worldSpaceX, worldSpaceY);
        }

        public Vector2 WebcamToWorld(double x, double y)
        {
            int webcamX = webCamTextureToMatHelper.GetWidth();
            int webCamXHalf = webcamX / 2;

            float tempX = (float)x;
            float worldSpaceX = 0;
            worldSpaceX = (tempX / webcamX) * (webCamXHalf - (-webCamXHalf)) - webCamXHalf;


            int webcamY = webCamTextureToMatHelper.GetHeight();
            int webCamYHalf = webcamY / 2;

            float tempY = (float)(y);
            float worldSpaceY = 0;
            worldSpaceY = (1 - tempY / webcamY) * (webCamYHalf - (-webCamYHalf)) - webCamYHalf;

            return new Vector2(worldSpaceX, worldSpaceY);
        }

        public Vector2 LocationBetweenPoints(Point start, Point end)
        {
            Vector2 startPosition = WebcamToWorld(start);
            Vector2 endPosition = WebcamToWorld(end);

            Vector2 midPosition = new Vector2((startPosition.x + endPosition.x) / 2, (startPosition.y + endPosition.y) / 2);

            return midPosition;
        }

        public static float DistanceBetweenPoints(Point start, Point end)
        {
            return Vector2.Distance(new Vector2((float)start.x, (float)start.y), new Vector2((float)end.x, (float)end.y));
        }

        public static float CalculateRotationAngle(Point start, Point end)
        {
            float angle = Mathf.Atan2((float)end.y - (float)start.y, (float)end.x - (float)start.x) * Mathf.Rad2Deg;
            return angle;
        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("OpenCVForUnityExample");
        }

        /// <summary>
        /// Raises the play button click event.
        /// </summary>
        public void OnPlayButtonClick()
        {
            webCamTextureToMatHelper.Play();
        }

        /// <summary>
        /// Raises the pause button click event.
        /// </summary>
        public void OnPauseButtonClick()
        {
            webCamTextureToMatHelper.Pause();
        }

        /// <summary>
        /// Raises the stop button click event.
        /// </summary>
        public void OnStopButtonClick()
        {
            webCamTextureToMatHelper.Stop();
        }

        /// <summary>
        /// Raises the change camera button click event.
        /// </summary>
        public void OnChangeCameraButtonClick()
        {
            webCamTextureToMatHelper.requestedIsFrontFacing = !webCamTextureToMatHelper.requestedIsFrontFacing;
        }
    }
}

#endif

#endif