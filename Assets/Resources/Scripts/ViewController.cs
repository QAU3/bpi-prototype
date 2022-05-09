using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
//using UnityEngine.iOS;

using PDollarGestureRecognizer;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ViewController : MonoBehaviour
{
    const int _TOUCH_ = 0;
    const int _MOUSE_ = 1;

    public GameObject cursor;
    public Button returnBtn; 
    public Transform gestureOnScreenPrefab;
    public Camera camera; 
    private TrailRenderer trail;
    private SpriteRenderer sprite;
    bool isFullScreen;
    float gesture_confidce;
    Color current_color; 

    private List<Gesture> trainingSet = new List<Gesture>();

    private List<Point> points = new List<Point>();
    private int strokeId = -1;

    private Vector3 virtualKeyPosition = Vector2.zero;
    private Rect drawArea;

    private RuntimePlatform platform;
    private int vertexCount = 0;

    private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
    private LineRenderer currentGestureLineRenderer;

    //GUI
    private string message;
    private bool recognized;
    private string newGestureName = "";

  
    void Start()
    {
    
        platform = Application.platform;
        drawArea = new Rect(0, 0, Screen.width, Screen.height);
        current_color=Color.white;
        //Load pre-made gestures
        TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
        foreach (TextAsset gestureXml in gesturesXml)
            trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));

        //Load user custom gestures
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (string filePath in filePaths)
            trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));


        //Get components from cursos
        trail= cursor.GetComponent<TrailRenderer>();
        sprite = cursor.GetComponent<SpriteRenderer>();


        ///Init button and position
        
        returnBtn.onClick.AddListener(ChangeScene);

    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("hub");
    }


    // Update is called once per frame
    void Update()
    {
        int INPUT_TYPE = _MOUSE_;
        if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                virtualKeyPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
            }
        }

        if (drawArea.Contains(virtualKeyPosition))
        {

            if (Input.GetMouseButtonDown(0))
            {

                if (recognized)
                {

                    recognized = false;
                    strokeId = -1;

                    points.Clear();

                    foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                    {

                        lineRenderer.SetVertexCount(0);
                        Destroy(lineRenderer.gameObject);
                    }

                    gestureLinesRenderer.Clear();
                }

                ++strokeId;

                Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation) as Transform;
                currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();

                gestureLinesRenderer.Add(currentGestureLineRenderer);

                vertexCount = 0;
            }

            if (Input.GetMouseButton(0))
            {
                points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));

                currentGestureLineRenderer.SetVertexCount(++vertexCount);
                currentGestureLineRenderer.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
            }
        }
        if (Input.touchCount > 0)
        {
            INPUT_TYPE = _TOUCH_;            
        }
       

        TriggerGestureRecognition(INPUT_TYPE);

    }

    void TriggerGestureRecognition(int INPUT_TYPE)
    {

        if (INPUT_TYPE == _TOUCH_)
        {
            Touch fireTouch = Input.GetTouch(0);
            if(fireTouch.phase == TouchPhase.Ended || fireTouch.phase == TouchPhase.Canceled)
            {
                RecognizeGesture();

            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                RecognizeGesture();
            }

        }
    }

    public void RecognizeGesture()
    {
        try
        {
            recognized = true;
            gesture_confidce = 0.8f;

            Gesture candidate = new Gesture(points.ToArray());
            Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

            message = gestureResult.GestureClass + " " + gestureResult.Score;

            if (gestureResult.GestureClass == "r" && gestureResult.Score > gesture_confidce)
            {
                current_color = Color.red;


                Debug.Log("catch" + current_color);
            }
            if (gestureResult.GestureClass == "b" && gestureResult.Score > gesture_confidce)
            {
                current_color = Color.blue;
                Debug.Log("catch" + current_color);

            }
            if (gestureResult.GestureClass == "g" && gestureResult.Score > gesture_confidce)
            {
                current_color = Color.green;
                Debug.Log("catch" + current_color);

            }
            if (gestureResult.GestureClass == "full" && gestureResult.Score > gesture_confidce + 0.1f)
            {

                if (isFullScreen)
                {
                    camera.backgroundColor = current_color;
                }
                else
                {
                    camera.backgroundColor = Color.black;

                }

                isFullScreen = !isFullScreen;
            }
            if (gestureResult.GestureClass == "play" && gestureResult.Score > gesture_confidce + 0.1f)
            {
                current_color = Color.white;
                Debug.Log("catch" + current_color);


            }

            // A simple 2 color gradient with a fixed alpha of 1.0f.
            float alpha = 1.0f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(current_color, 0.0f), new GradientColorKey(current_color, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
            sprite.color = current_color;
            trail.colorGradient = gradient;
            returnBtn.GetComponent<Image>().color = current_color;

            // sphere.GetComponent<RawImage>().color = current_color;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    void OnGUI()
    {

        GUI.Box(drawArea, "Draw Area");

//        GUI.Label(new Rect(10, Screen.height - 40, 500, 50), message);

//        if (GUI.Button(new Rect(Screen.width - 100, 10, 100, 30), "Recognize"))
//        {

//            recognized = true;

//            Gesture candidate = new Gesture(points.ToArray());
//            Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

//            message = gestureResult.GestureClass + " " + gestureResult.Score;
//        }

//        GUI.Label(new Rect(Screen.width - 200, 150, 70, 30), "Add as: ");
//        newGestureName = GUI.TextField(new Rect(Screen.width - 150, 150, 100, 30), newGestureName);

//        if (GUI.Button(new Rect(Screen.width - 50, 150, 50, 30), "Add") && points.Count > 0 && newGestureName != "")
//        {

//            string fileName = String.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, newGestureName, DateTime.Now.ToFileTime());

//#if !UNITY_WEBPLAYER
//            GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
//#endif

//            trainingSet.Add(new Gesture(points.ToArray(), newGestureName));

//            newGestureName = "";
//        }
    } 

}
