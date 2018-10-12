using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using PDollarGestureRecognizer;

public class DemoEdit : MonoBehaviour
{
    public int stroke_limit;//when reached, destroys lines
    public Transform gestureOnScreenPrefab;

    Staller staller;
    Note note;

    private List<Gesture> trainingSet = new List<Gesture>();

    private List<Point> points = new List<Point>();
    private int strokeId = -1;

    private Vector3 virtualKeyPosition = Vector2.zero;
    private Rect drawArea;

    private RuntimePlatform platform;
    private int vertexCount = 0;

    private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
    private LineRenderer currentGestureLineRenderer;

    private string demo_stroke_name;//name of correct stroke
    private int stroke_count;//amount of strokes
    
    //GUI
    private string message;
    private bool recognized;
    private string newGestureName = "";

    void Start()
    {
        stroke_count = 0;
        platform = Application.platform;
        drawArea = new Rect(0, 0, Screen.width - Screen.width / 3, Screen.height);
        
        //Load pre-made gestures
        TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
        foreach (TextAsset gestureXml in gesturesXml)
            trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));

        //Load user custom gestures
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (string filePath in filePaths)
            trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));
    }

    void Update()
    {

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
                message = "";
                if (recognized)
                {

                    recognized = false;
                    strokeId = -1;
                    
                    points.Clear();

                    stroke_count++;

                    if(stroke_count == stroke_limit)
                    {
                        foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                        {

                            lineRenderer.SetVertexCount(0);
                            Destroy(lineRenderer.gameObject);
                        }

                        gestureLinesRenderer.Clear();

                        stroke_count = 0;
                    }
                    
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
    }

    void OnGUI()
    {

        GUI.Box(drawArea, "Name");

        GUI.Label(new Rect(10, Screen.height - 40, 500, 50), message);

        if (Input.GetMouseButtonUp(0))
        {
            
            if(stroke_count == 0)
                demo_stroke_name = "Hito1";
            else
                demo_stroke_name = "HitoR";
           
            recognized = true;

            Gesture candidate = new Gesture(points.ToArray()); //Gio: gesture to check
            Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());//Gio: is the result after hitting recognize

            //message = gestureResult.GestureClass + " " + gestureResult.Score; //Gio: gestureResult.GestureClass is name of gesture recognized

            if (gestureResult.GestureClass == demo_stroke_name && gestureResult.Score > .90)
            {
                message = "CORRECT";
            }
            else message = "WRONG";

        }

        GUI.Label(new Rect(Screen.width - 200, 150, 70, 30), "Add as: ");
        newGestureName = GUI.TextField(new Rect(Screen.width - 150, 150, 100, 30), newGestureName);

        if (GUI.Button(new Rect(Screen.width - 50, 150, 50, 30), "Add") && points.Count > 0 && newGestureName != "")
        {

            string fileName = String.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, newGestureName, DateTime.Now.ToFileTime());

#if !UNITY_WEBPLAYER
            GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
#endif

            trainingSet.Add(new Gesture(points.ToArray(), newGestureName));

            newGestureName = "";
        }
    }
}
